USE master
GO

--drop database if it exists
IF DB_ID('final_capstone') IS NOT NULL
BEGIN
	ALTER DATABASE final_capstone SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE final_capstone;

END

CREATE DATABASE final_capstone
GO

USE final_capstone
GO

--create tables
CREATE TABLE users (
	user_id int IDENTITY(1,1) NOT NULL,
	username varchar(50) NOT NULL,
	password_hash varchar(200) NOT NULL,
	salt varchar(200) NOT NULL,
	user_role varchar(50) NOT NULL
	CONSTRAINT PK_user PRIMARY KEY (user_id)
)

CREATE TABLE photos (
	photo_id int IDENTITY(1,1) NOT NULL,
	photo varchar(MAX) NOT NULL,
	photo_name varchar(200),
	CONSTRAINT PK_photo PRIMARY KEY (photo_id)
)

CREATE TABLE posts (
	post_id int IDENTITY(1,1) NOT NULL,
	user_id int NOT NULL,
	photo_id int NOT NULL,
	caption varchar(MAX),
	likes int,
	upload_time datetime DEFAULT  getdate(), 
	CONSTRAINT PK_post PRIMARY KEY (post_id),
	CONSTRAINT FK_posts_users FOREIGN KEY (user_id) REFERENCES users (user_id),
	CONSTRAINT FK_posts_photos FOREIGN KEY (photo_id) REFERENCES photos (photo_id)
)

CREATE TABLE comments (
	comment_id int IDENTITY(1,1) NOT NULL,
	user_id int NOT NULL,
	post_id int NOT NULL,
	comment varchar(MAX) NOT NULL
	CONSTRAINT PK_comment PRIMARY KEY (comment_id),
	CONSTRAINT FK_comments_users FOREIGN KEY (user_id) REFERENCES users (user_id),
	CONSTRAINT FK_comments_post FOREIGN KEY (post_id) REFERENCES posts (post_id)
)

CREATE TABLE liked_posts (
	liked_id int identity(1,1) NOT NULL,
	post_id int NOT NULL,
	user_id int NOT NULL,
	liked bit NOT NULL
	CONSTRAINT PK_post_liked PRIMARY KEY (liked_id),	
	CONSTRAINT FK_liked_posts_users FOREIGN KEY (user_id) REFERENCES users (user_id),
	CONSTRAINT FK_liked_posts_posts FOREIGN KEY (post_id) REFERENCES posts (post_id)
)
	
CREATE TABLE favorited_posts (
	favorited_id int identity(1,1) NOT NULL,
	post_id int NOT NULL,
	user_id int NOT NULL,
	favorited bit NOT NULL
	CONSTRAINT PK_post_favorited PRIMARY KEY (favorited_id),	
	CONSTRAINT FK_favorited_posts_users FOREIGN KEY (user_id) REFERENCES users (user_id),
	CONSTRAINT FK_favorited_posts_posts FOREIGN KEY (post_id) REFERENCES posts (post_id)
)

--populate default data
INSERT INTO users (username, password_hash, salt, user_role) VALUES ('user','Jg45HuwT7PZkfuKTz6IB90CtWY4=','LHxP4Xh7bN0=','user');
INSERT INTO users (username, password_hash, salt, user_role) VALUES ('admin','YhyGVQ+Ch69n4JMBncM4lNF/i9s=', 'Ar/aB2thQTI=','admin');

GO