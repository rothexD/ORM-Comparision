drop Table if exists Books cascade;
drop Table if exists Chapters cascade;
drop Table if exists Pages cascade;
drop Table if exists fk_articles_bills cascade;
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
    fk_knight_id int
);
Create Table Knight
(
    Id int primary key,
    Name Text,
    fk_Weapon_Id int,
    foreign key (fk_Weapon_Id) references Weapon(id)
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
Create Table fk_articles_bills
(
    fk_billsId int NOT NULL,
    fk_articlesId int NOT NULL,
    foreign key (fk_articlesId) references Articles(articlesID) on delete cascade,
    foreign key (fk_billsId) references Bills(billsID) on delete cascade,
    primary key (fk_billsId,fk_articlesId)
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
    fk_books_bookid int not null,
    foreign key (fk_books_bookid) references Books(bookID)
);
Create Table Pages(
    pagesid int primary key not null,
    fk_chapters_chapterId int not null,
    text text not null,
    foreign key (fk_chapters_chapterId) references Chapters(chapterID)
);
select count(*) from pages;
