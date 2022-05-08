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
    fk_knightid int
);
Create Table Knight
(
    Id int primary key,
    Name Text,
    fk_WeaponId int,
    foreign key (fk_WeaponId) references Weapon(id)
);

Create Table Customers
(
    customerID     int primary Key,
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
    billsID int primary Key,
    purchasePrice numeric NOT NULL
);
Create Table Articles(
    articlesID int primary key,
    articleName text NOT NULL,
    articlePrice numeric NOT NULL,
    isHidden bool
);
Create Table BillsArticles
(
    FK_BillsID int NOT NULL,
    FK_ArticlesId int NOT NULL,
    foreign key (FK_ArticlesId) references Articles(articlesID) on delete cascade ,
    foreign key (FK_BillsID) references Bills(billsID) on delete cascade,
    primary key (FK_BillsID,FK_ArticlesId)
);

Create Table Books(
    bookID int primary key,
    bookName text not null,
    price numeric not null,
    authorname text not null
);
Create Table Chapters(
    chapterID int primary key,
    chapterName text not null,
    FK_bookID int not null,
    foreign key (FK_bookID) references Books(bookID)
);
Create Table Pages(
    PagesID int primary key not null,
    fk_chapter_Id int not null,
    Text text not null,
    foreign key (fk_chapter_Id) references Chapters(chapterID)
);
