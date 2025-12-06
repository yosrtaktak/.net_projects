USE CarRentalDB;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

CREATE INDEX IX_Rentals_UserId ON Rentals(UserId);
CREATE UNIQUE INDEX IX_AspNetUsers_DriverLicenseNumber ON AspNetUsers(DriverLicenseNumber) WHERE DriverLicenseNumber IS NOT NULL;
ALTER TABLE Rentals ADD CONSTRAINT FK_Rentals_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id);

PRINT 'Constraints created successfully';
GO
