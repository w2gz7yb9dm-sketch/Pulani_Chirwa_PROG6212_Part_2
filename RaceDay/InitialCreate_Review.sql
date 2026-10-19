IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [FirstName] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [PasswordHash] nvarchar(500) NOT NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [Role] nvarchar(20) NOT NULL,
    [DateOfBirth] date NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);

CREATE TABLE [Events] (
    [EventId] int NOT NULL IDENTITY,
    [EventName] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NULL,
    [EventDate] datetime2 NOT NULL,
    [Location] nvarchar(max) NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Events] PRIMARY KEY ([EventId]),
    CONSTRAINT [FK_Events_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);

CREATE TABLE [Categories] (
    [CategoryId] int NOT NULL IDENTITY,
    [EventId] int NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [DistanceKm] decimal(6,2) NOT NULL,
    [EntryFee] decimal(10,2) NOT NULL,
    [MinimumAge] tinyint NULL,
    [Capacity] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([CategoryId]),
    CONSTRAINT [FK_Categories_Events_EventId] FOREIGN KEY ([EventId]) REFERENCES [Events] ([EventId]) ON DELETE CASCADE
);

CREATE TABLE [Routes] (
    [RouteId] int NOT NULL IDENTITY,
    [EventId] int NOT NULL,
    [RouteName] nvarchar(150) NOT NULL,
    [StartLatitude] decimal(9,6) NOT NULL,
    [StartLongitude] decimal(9,6) NOT NULL,
    [MapUrl] nvarchar(500) NULL,
    [ElevationGainM] int NULL,
    [RouteNotes] nvarchar(1000) NULL,
    CONSTRAINT [PK_Routes] PRIMARY KEY ([RouteId]),
    CONSTRAINT [FK_Routes_Events_EventId] FOREIGN KEY ([EventId]) REFERENCES [Events] ([EventId]) ON DELETE CASCADE
);

CREATE TABLE [Enrolments] (
    [EnrolmentId] int NOT NULL IDENTITY,
    [ParticipantId] int NOT NULL,
    [EventId] int NOT NULL,
    [CategoryId] int NOT NULL,
    [RaceNumber] nvarchar(20) NULL,
    [EnrolledAt] datetime2 NOT NULL,
    [PaymentStatus] nvarchar(20) NOT NULL,
    [EnrolmentStatus] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Enrolments] PRIMARY KEY ([EnrolmentId]),
    CONSTRAINT [FK_Enrolments_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Enrolments_Events_EventId] FOREIGN KEY ([EventId]) REFERENCES [Events] ([EventId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Enrolments_Users_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);

CREATE TABLE [Results] (
    [ResultId] int NOT NULL IDENTITY,
    [EnrolmentId] int NOT NULL,
    [FinishTime] time NULL,
    [OverallPosition] int NULL,
    [CategoryPosition] int NULL,
    [ResultStatus] nvarchar(20) NOT NULL,
    [CapturedAt] datetime2 NOT NULL,
    [CapturedByUserId] int NOT NULL,
    CONSTRAINT [PK_Results] PRIMARY KEY ([ResultId]),
    CONSTRAINT [FK_Results_Enrolments_EnrolmentId] FOREIGN KEY ([EnrolmentId]) REFERENCES [Enrolments] ([EnrolmentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Results_Users_CapturedByUserId] FOREIGN KEY ([CapturedByUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_Categories_EventId_Name] ON [Categories] ([EventId], [Name]);

CREATE INDEX [IX_Enrolments_CategoryId] ON [Enrolments] ([CategoryId]);

CREATE INDEX [IX_Enrolments_EventId] ON [Enrolments] ([EventId]);

CREATE UNIQUE INDEX [IX_Enrolments_ParticipantId_EventId] ON [Enrolments] ([ParticipantId], [EventId]);

CREATE INDEX [IX_Events_UserId] ON [Events] ([UserId]);

CREATE INDEX [IX_Results_CapturedByUserId] ON [Results] ([CapturedByUserId]);

CREATE UNIQUE INDEX [IX_Results_EnrolmentId] ON [Results] ([EnrolmentId]);

CREATE UNIQUE INDEX [IX_Routes_EventId] ON [Routes] ([EventId]);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261019073718_InitialCreate', N'10.0.13');

COMMIT;
GO

