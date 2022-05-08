drop Table if exists Books cascade;
drop Table if exists Chapters cascade;
drop Table if exists Pages cascade;
drop Table if exists BillsArticles cascade;
drop Table if exists Articles cascade;
drop Table if exists Bills cascade;
drop Table if exists Customers cascade;


Create Table Customers
(
    customerID     int primary Key,
	lastname char(50) NOT NULL,
    customer_since TimeStamp NOT NULL,
    email          char(50)  NOT NULL,
    isVip          bool      NOT NULL,
    customerLikesColorGreen bool,
    customerLikesCars bool,
    totalArticlesAddedToShoppingCart int NOT NULL,
    defaultPurchasePriceMultiplicator numeric NOT NULL
);

Create Table Bills
(
    billsID int primary Key,
    purchasePrice numeric NOT NULL,
    purcahseDate timestamp NOT NULL
);
Create Table Articles(
    articlesID int primary key,
    articleName char(50) NOT NULL,
    articlePrice numeric NOT NULL,
    isHidden bool
);
Create Table BillsArticles
(
    FK_BillsID int NOT NULL,
    FK_ArticlesId int NOT NULL,
    foreign key (FK_ArticlesId) references Articles(articlesID)on delete cascade,
    foreign key (FK_BillsID) references Bills(billsID) on delete cascade,
    primary key (FK_BillsID,FK_ArticlesId)
);

Create Table Books(
    bookID int primary key,
    bookName varchar(150) not null,
    price numeric not null,
    authorname varchar(150) not null
);
Create Table Chapters(
    chapterID int primary key,
    chapterName text not null,
    fk_bookid_id_fk int not null,
    foreign key (fk_bookid_id_fk) references Books(bookID) on delete cascade
);
Create Table Pages(
    PagesID int primary key not null,
    fk_chapter_Id_id_fk int not null,
    Text text not null,
    foreign key (fk_chapter_Id_id_fk) references Chapters(chapterID) on delete cascade
);
select count(*) from pages;
select count(*) from chapters;
select count(*) from books;
