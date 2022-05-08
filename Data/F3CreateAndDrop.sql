drop Table if exists Books cascade;
drop Table if exists Chapters cascade;
drop Table if exists Pages cascade;
drop Table if exists BillsArticles cascade;
drop Table if exists Articles cascade;
drop Table if exists Bills cascade;
drop Table if exists Customers cascade;

drop Table if exists Knight cascade;
drop Table if exists Weapon cascade;

Create Table Weapon
(
    Id int primary key,
    WeaponName Text,
    Damage int,
    knight_id_fk int
);
Create Table Knight
(
    Id int primary key,
    Name Text,
    weapon_id_fk int,
    foreign key (weapon_id_fk) references Weapon(id)
);
Create Table Customers
(
    Id     int primary Key,
	lastname text NOT NULL,
    email          text  NOT NULL,
    isVip          bool      NOT NULL,
    customerLikesColorGreen bool,
    customerLikesCars bool,
    totalArticlesAddedToShoppingCart int NOT NULL,
    defaultPurchasePriceMultiplicator numeric NOT NULL
);

Create Table Bills
(
    Id int primary Key,
    purchasePrice numeric NOT NULL
);
Create Table Articles(
    Id int primary key,
    articleName text NOT NULL,
    articlePrice numeric NOT NULL,
    isHidden bool
);
Create Table BillsArticles
(
    Bills_id_fk int NOT NULL,
    Articles_id_fk int NOT NULL,
    foreign key (Articles_id_fk) references Articles(Id) on delete cascade,
    foreign key (Bills_id_fk) references Bills(Id)on delete cascade,
    primary key (Bills_id_fk,Articles_id_fk)
);

Create Table Books(
    Id int primary key,
    bookName text not null,
    price numeric not null,
    authorname text not null
);
Create Table Chapters(
    Id int primary key,
    chapterName text not null,
    Book_id_fk int not null,
    foreign key (book_id_fk) references Books(Id)
);
Create Table Pages(
    Id int primary key not null,
    Chapter_id_fk int not null,
    Text text not null,
    foreign key (Chapter_id_fk) references Chapters(Id)
);
