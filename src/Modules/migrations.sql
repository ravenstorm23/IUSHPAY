CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "InstitutionalCode" text NOT NULL,
    "FullName" text NOT NULL,
    "Email" text NOT NULL,
    "CarnetNumber" text NOT NULL,
    "PasswordHash" text NOT NULL,
    "Role" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "ParkingAccesses" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Method" integer NOT NULL,
    "IsAuthorized" boolean NOT NULL,
    "AccessedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ParkingAccesses" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ParkingAccesses_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Wallets" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Balance" numeric(18,2) NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Wallets" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Wallets_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Transactions" (
    "Id" uuid NOT NULL,
    "WalletId" uuid NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "Status" integer NOT NULL,
    "Type" integer NOT NULL,
    "Reference" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Transactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Transactions_Wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "Wallets" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_ParkingAccesses_UserId" ON "ParkingAccesses" ("UserId");

CREATE INDEX "IX_Transactions_WalletId" ON "Transactions" ("WalletId");

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

CREATE UNIQUE INDEX "IX_Wallets_UserId" ON "Wallets" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260513012738_InitialCreate', '8.0.0');

COMMIT;

