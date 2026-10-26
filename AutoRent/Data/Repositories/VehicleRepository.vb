
Imports System.Data
Imports System.IO
Imports AutoRent.Data.Context

Namespace Data.Repositories

    Public NotInheritable Class VehicleRepository

        Private Sub New()
        End Sub

        Public Shared ReadOnly Statuses As String() = {
            "Available",
            "Reserved",
            "Rented",
            "Maintenance",
            "Retired"
        }

        ' ==========================================
        ' LOAD VEHICLES
        ' ==========================================

        Public Shared Function GetAll() As DataTable

            Return Db.Query(
                "SELECT v.[vehicle_id], v.[plate], v.[make], " &
                "v.[model], v.[category], v.[model_year], " &
                "v.[mileage], v.[status], v.[image_path], " &
                "r.[daily_rate] " &
                "FROM [Vehicles] AS v " &
                "LEFT JOIN [Rates] AS r " &
                "ON v.[category] = r.[category] " &
                "ORDER BY v.[plate]")

        End Function

        Public Shared Function GetPickList() As DataTable

            Return Db.Query(
                "SELECT [vehicle_id], " &
                "[plate] & ' - ' & [make] & ' ' & [model] " &
                "AS [display] FROM [Vehicles] " &
                "WHERE [status] <> 'Retired' " &
                "ORDER BY [plate]")

        End Function

        ' ==========================================
        ' MAIN VEHICLE IMAGE FOLDER
        ' ==========================================

        Public Shared Function GetImageFolder() As String

            Dim dataFolder As String =
                Path.GetDirectoryName(
                    AccessDatabaseConnection.GetDatabasePath()
                )

            Return Path.GetFullPath(
                Path.Combine(
                    dataFolder,
                    "..",
                    "Images",
                    "Vehicles"
                )
            )

        End Function

        ' ==========================================
        ' CATEGORY FOLDER NAMES
        ' ==========================================

        Public Shared Function GetCategoryFolderName(
            category As String
        ) As String

            If String.IsNullOrWhiteSpace(category) Then
                Return "Other"
            End If

            Select Case category.Trim().ToLowerInvariant()

                Case "sedan"
                    Return "1. Sedan"

                Case "suv (sport utility vehicle)", "suv"
                    Return "2. SUV (Sport Utility Vehicle)"

                Case "hatchback"
                    Return "3. Hatchback"

                Case "van"
                    Return "4. Van"

                Case "mpv (multi-purpose vehicle)", "mpv"
                    Return "5. MPV (Multi-Purpose Vehicle)"

                Case "pickup truck"
                    Return "6. Pickup Truck"

                Case "sports car"
                    Return "7. Sports Car"

                Case "luxury car"
                    Return "8. Luxury Car"

                Case "hybrid car"
                    Return "9. Hybrid Car"

                Case "electric car (ev)"
                    Return "10. Electric Car (EV)"

                Case Else
                    Return "Other"

            End Select

        End Function

        ' ==========================================
        ' GET FULL IMAGE PATH
        ' Supports both old filenames and subfolders.
        ' ==========================================

        Public Shared Function GetImageFilePath(
            imagePath As String
        ) As String

            If String.IsNullOrWhiteSpace(imagePath) Then
                Return Nothing
            End If

            Try

                Dim relativePath As String =
                    imagePath.Trim().
                    Replace("/"c, Path.DirectorySeparatorChar).
                    Replace("\"c, Path.DirectorySeparatorChar)

                If Path.IsPathRooted(relativePath) OrElse
                   relativePath.Contains(":"c) Then

                    Return Nothing

                End If

                Dim rootFolder As String =
                    Path.GetFullPath(GetImageFolder()).
                    TrimEnd(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar
                    ) & Path.DirectorySeparatorChar

                Dim fullPath As String =
                    Path.GetFullPath(
                        Path.Combine(rootFolder, relativePath)
                    )

                ' Prevent paths from escaping Vehicles.
                If Not fullPath.StartsWith(
                    rootFolder,
                    StringComparison.OrdinalIgnoreCase
                ) Then

                    Return Nothing

                End If

                Return fullPath

            Catch ex As Exception
                Return Nothing
            End Try

        End Function

        ' ==========================================
        ' SAVE IMAGE TO CORRECT CATEGORY FOLDER
        ' ==========================================

        Public Shared Function SaveImageToCategory(
            sourceFilePath As String,
            category As String
        ) As String

            If String.IsNullOrWhiteSpace(sourceFilePath) Then
                Return Nothing
            End If

            Dim imageInfo As New FileInfo(sourceFilePath)

            If Not imageInfo.Exists Then
                Throw New FileNotFoundException(
                    "The selected vehicle photo was not found."
                )
            End If

            If imageInfo.Length > 5L * 1024L * 1024L Then
                Throw New IOException(
                    "The vehicle photo must be 5 MB or smaller."
                )
            End If

            Dim extension As String =
                imageInfo.Extension.ToLowerInvariant()

            Select Case extension

                Case ".jpg", ".jpeg", ".png", ".bmp"
                    ' Supported image extension.

                Case Else
                    Throw New IOException(
                        "Only JPG, JPEG, PNG and BMP images are supported."
                    )

            End Select

            Dim categoryFolder As String =
                GetCategoryFolderName(category)

            Dim destinationFolder As String =
                Path.Combine(
                    GetImageFolder(),
                    categoryFolder
                )

            ' Create category folder if missing.
            Directory.CreateDirectory(destinationFolder)

            ' Unique filename prevents overwriting photos.
            Dim newFileName As String =
                Guid.NewGuid().ToString("N") & extension

            Dim destinationPath As String =
                Path.Combine(
                    destinationFolder,
                    newFileName
                )

            File.Copy(
                imageInfo.FullName,
                destinationPath,
                False
            )

            ' Store only the relative path in Access.
            Return Path.Combine(
                categoryFolder,
                newFileName
            )

        End Function

        ' ==========================================
        ' CLEAN UP UNUSED GENERATED IMAGES
        ' Never delete manually named pictures.
        ' ==========================================

        Public Shared Sub DeleteUnusedImage(
            imagePath As String
        )

            Try

                Dim fullPath As String =
                    GetImageFilePath(imagePath)

                If String.IsNullOrWhiteSpace(fullPath) Then
                    Return
                End If

                Dim fileName As String =
                    Path.GetFileNameWithoutExtension(fullPath)

                Dim generatedId As Guid

                ' Only remove AutoRent-generated GUID files.
                If Not Guid.TryParseExact(
                    fileName,
                    "N",
                    generatedId
                ) Then

                    Return

                End If

                Dim referenceCount As Integer =
                    Db.Count(
                        "SELECT COUNT(*) FROM [Vehicles] " &
                        "WHERE [image_path] = ?",
                        Db.Text(imagePath, 255)
                    )

                If referenceCount > 0 Then
                    Return
                End If

                If File.Exists(fullPath) Then
                    File.Delete(fullPath)
                End If

            Catch ex As Exception
                ' Never undo a successful database operation
                ' because image cleanup failed.
            End Try

        End Sub

        ' ==========================================
        ' ADD VEHICLE
        ' ==========================================

        Public Shared Function Add(
            plate As String,
            make As String,
            model As String,
            category As String,
            modelYear As Integer?,
            mileage As Integer,
            status As String,
            Optional imageFileName As String = Nothing
        ) As String

            plate = plate.Trim().ToUpperInvariant()

            If PlateExists(plate, 0) Then
                Return $"A vehicle with plate {plate} already exists."
            End If

            Db.Execute(
                "INSERT INTO [Vehicles] " &
                "([plate], [make], [model], [category], " &
                "[model_year], [mileage], [status], " &
                "[image_path], [created_at]) " &
                "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
                Db.Text(plate, 20),
                Db.Text(make, 50),
                Db.Text(model, 50),
                Db.Text(category, 50),
                Db.IntOrNull(modelYear),
                Db.Int(mileage),
                Db.Text(status, 20),
                Db.Text(imageFileName, 255),
                Db.Dt(DateTime.Now)
            )

            Return Nothing

        End Function

        ' ==========================================
        ' UPDATE VEHICLE
        ' ==========================================

        Public Shared Function Update(
            vehicleId As Integer,
            plate As String,
            make As String,
            model As String,
            category As String,
            modelYear As Integer?,
            mileage As Integer,
            status As String,
            Optional imageFileName As String = Nothing
        ) As String

            plate = plate.Trim().ToUpperInvariant()

            If PlateExists(plate, vehicleId) Then
                Return $"Another vehicle already uses plate {plate}."
            End If

            Db.Execute(
                "UPDATE [Vehicles] SET " &
                "[plate] = ?, " &
                "[make] = ?, " &
                "[model] = ?, " &
                "[category] = ?, " &
                "[model_year] = ?, " &
                "[mileage] = ?, " &
                "[status] = ?, " &
                "[image_path] = ? " &
                "WHERE [vehicle_id] = ?",
                Db.Text(plate, 20),
                Db.Text(make, 50),
                Db.Text(model, 50),
                Db.Text(category, 50),
                Db.IntOrNull(modelYear),
                Db.Int(mileage),
                Db.Text(status, 20),
                Db.Text(imageFileName, 255),
                Db.Int(vehicleId)
            )

            Return Nothing

        End Function

        ' ==========================================
        ' DELETE VEHICLE
        ' ==========================================

        Public Shared Function Delete(
            vehicleId As Integer
        ) As String

            Dim uses As Integer =
                Db.Count(
                    "SELECT COUNT(*) FROM [Maintenance] " &
                    "WHERE [vehicle_id] = ?",
                    Db.Int(vehicleId)
                ) +
                Db.Count(
                    "SELECT COUNT(*) FROM [Reservations] " &
                    "WHERE [vehicle_id] = ?",
                    Db.Int(vehicleId)
                )

            If uses > 0 Then

                Return "This vehicle has maintenance or " &
                       "reservation history, so it can't be deleted. " &
                       "Set its status to Retired instead."

            End If

            Db.Execute(
                "DELETE FROM [Vehicles] WHERE [vehicle_id] = ?",
                Db.Int(vehicleId)
            )

            Return Nothing

        End Function

        ' ==========================================
        ' CHECK DUPLICATE PLATE
        ' ==========================================

        Private Shared Function PlateExists(
            plate As String,
            excludeId As Integer
        ) As Boolean

            Return Db.Count(
                "SELECT COUNT(*) FROM [Vehicles] " &
                "WHERE [plate] = ? AND [vehicle_id] <> ?",
                Db.Text(plate, 20),
                Db.Int(excludeId)
            ) > 0

        End Function

    End Class

End Namespace
