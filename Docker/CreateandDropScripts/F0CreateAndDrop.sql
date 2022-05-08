drop Table if exists Books cascade;
drop Table if exists Chapters cascade;
drop Table if exists Pages cascade;
drop Table if exists BillsArticles cascade;
drop Table if exists Articles cascade;
drop Table if exists Bills cascade;
drop Table if exists AdditionalCustomerInformation cascade;
drop Table if exists Customers cascade;
Create Table AdditionalCustomerInformation
(
    additionalCustomerInformationId int primary key,
    customerLikesColorGreen bool,
    customerLikesCars bool,
    totalArticlesAddedToShoppingCart int NOT NULL,
    defaultPurchasePriceMultiplicator numeric NOT NULL
);
Create Table Customers
(
    customerID     int primary Key,
    FK_AdditionalCustomerInformation int not null,
	lastname char(50) NOT NULL,
    customer_since TimeStamp NOT NULL,
    email          char(50)  NOT NULL,
    isVip          bool      NOT NULL,
    foreign key (FK_AdditionalCustomerInformation) references additionalcustomerinformation(additionalCustomerInformationId)
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
    FK_bookID int not null,
    foreign key (chapterID) references Books(bookID) on delete cascade
);
Create Table Pages(
    PagesID int primary key not null,
    fk_chapter_Id int not null,
    Text text not null,
    foreign key (fk_chapter_Id) references Chapters(chapterID) on delete cascade
);
