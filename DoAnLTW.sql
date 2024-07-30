create database DB_BANKHOAHOC_ONLINE

use DB_BANKHOAHOC_ONLINE

go

-- Tạo bảng Người dùng (Users)


create table NguoiDung
(
	MaND varchar(11) not null,
	TenND nvarchar(50) not null,
	MatKhau varchar(255) not null,
	HoTen nvarchar(100),
	Email varchar(255) not null,
	DienThoai varchar(11),	
	Roles BIT,
	constraint pk_nguoidung primary key (MaND)
)

-- Tạo bảng Khóa học (Courses)


create table KhoaHoc
(
	MaKH varchar(11) not null,
	TenKH nvarchar(50) not null,
	MoTaKH nvarchar(255),
	GiangVien nvarchar(50) not null,
	NgayBD date,
	NgayKT date,
	GiaKhoaHoc money,
	TenMonHoc nvarchar(50),
	Picture varchar(50),
	TrangThai bit default 1,
	constraint pk_khoahoc primary key (MaKH)
)

-- Tạo bảng Bài giảng (Lessons)


create table BaiGiang
(
	MaBG varchar(11) not null,
	TieuDeBG nvarchar(255) not null,
	NoiDungBG nvarchar(255),
	MaKH varchar(11),
	ThuTuBaiHoc int,
	Video varchar(500),
	TrangThai bit default 1,
	constraint pk_baigiang primary key (MaBG)
)



-- Tạo bảng Bài tập (Assignments)


create table BaiTap
(
	MaBT varchar(11) not null,
	TieuDeBT nvarchar(255) not null,
	NoiDungBT nvarchar(255),
	HangNop date,
	MaKH varchar(11) not null,
	constraint pk_baitap primary key (MaBT)
)

-- Tạo bảng Kết quả (Results)


create table KetQua
(
	MaKQ varchar(11) not null,
	MaND varchar(11),
	MaBT varchar(11),
	Diem float,
	constraint pk_ketqua primary key (MaKQ, MaND, MaBT)
)



-- Tạo bảng Đánh giá (Reviews)


create table DanhGia
(
	MaDG varchar(11) not null,
	MaND varchar(11) not null,
	MaKH varchar(11) not null,
	DanhGia nvarchar(255),
	XepLoai nvarchar(15),
	NgayNopDanhGia date,
	constraint pk_danhgia primary key (MaDG, MaND, MaKH)
)


-- Tạo bảng Đăng ký khóa học (Enrollments)


create table HoaDonDky
(
	MaHD varchar(11) primary key,
	MaND varchar(11),
	NgayDangKy date default getdate(),
	TrangThai nvarchar(50) default N'Đang xử lý'
)


-- Tạo bảng Thanh toán (Payments)


create table ChiTietHoaDon
(
	MaHD varchar(11),
	MaKH varchar(11),
	SoTien money,
	PhuongThuc nvarchar(50),	
	constraint pk_thanhtoan primary key (MaHD, MaKH)
)


create table rateKhoaHoc
(
	ID int identity not null,
	Mota nvarchar(255),
	rateDate Date default getdate(),
	MaKH varchar(11),
	MaND varchar(11),
	constraint pk_rateKH primary key (ID)
)

create table CourseCart
(
	MaKH varchar(11),
	MaND varchar(11),
	TenKH nvarchar(255) not null,
	MoTaKH nvarchar(255),
	GiangVien nvarchar(100) not null,
	NgayBD date,
	NgayKT date,
	GiaKhoaHoc money,
	TenMonHoc nvarchar(255),
	Picture varchar(50),
	SoLuong int,
	primary key (MaKH, MaND),
	constraint FK_CourseCart_Course foreign key (MaKH) references KhoaHoc (MaKH),
	constraint FK_CourseCart_NguoiDung foreign key (MaND) references NguoiDung (MaND)
)
alter table rateKhoaHoc
add constraint fk_rateKH_KH foreign key (MaKH) references KhoaHoc(MaKH)

alter table rateKhoaHoc
add constraint fk_rateKH_ND foreign key (MaND) references NguoiDung(MaND)

--KHÓA NGOẠI 

alter table BaiGiang
add constraint fk_baigiang_khoahoc foreign key (MaKH) references KhoaHoc (MaKH)

alter table BaiTap
add constraint fk_baitap_khoahoc foreign key (MaKH) references KhoaHoc (MaKH)

alter table KetQua
add constraint fk_ketqua_baitap foreign key (MaBT) references BaiTap (MaBT)

alter table KetQua
add constraint fk_ketqua_nguoidung foreign key (MaND) references NguoiDung (MaND)

alter table DanhGia
add constraint fk_danhgia_nguoidung foreign key (MaND) references NguoiDung (MaND)

alter table DanhGia
add constraint fk_danhgia_khoahoc foreign key (MaKH) references KhoaHoc (MaKH)

alter table HoaDonDky
add constraint fk_hoadon_nguoidung foreign key (MaND) references NguoiDung (MaND)

alter table ChiTietHoaDon
add constraint fk_cthd_khoahoc foreign key (MaKH) references KhoaHoc (MaKH)

alter table ChiTietHoaDon
add constraint fk_cthd_dky foreign key (MaHD) references HoaDonDky(MaHD)

set dateformat dmy


INSERT INTO NguoiDung
VALUES 
('ND20230001','Quang123','$2a$12$2u81Uk4ww2sG1eMIEmRaveYDFN4G4rmkI3qP8yNb0LkLfjFBUBq2u',N'Dương Thuận Quang','dtq@gmail.com','0935684956',1)



set dateformat dmy

select * from KhoaHoc
insert into KhoaHoc(MaKH, TenKH, MoTaKH, GiangVien, NgayBD, NgayKT, GiaKhoaHoc, TenMonHoc, Picture)
values
('KH2023001',N'Khóa học Lập trình C',N'Khóa học này giúp bạn có thể tự lập trình được những phương thức xử lý dữ liệu bằng ngôn ngữ C','28tech',149000,N'Ngôn ngữ C','~/Content/Images/C.png'),
('KH2023002',N'Khóa học Lập trình C++',N'Khóa học này giúp bạn có thể tự lập trình được những phương thức xử lý dữ liệu bằng ngôn ngữ C++','28tech',169000,N'Ngôn ngữ C++','~/Content/Images/CPlus.png'),
('KH2023003',N'Khóa học Lập trình Java',N'Khóa học này giúp bạn có thể tự lập trình được những phương thức xử lý dữ liệu bằng ngôn ngữ Java','Kteam',169000,N'Ngôn ngữ Java','~/Content/Images/Java.png'),
('KH2023004',N'Khóa học Lập trình Python',N'Khóa học này giúp bạn có thể tự lập trình được những phương thức xử lý dữ liệu bằng ngôn ngữ Python','Kteam',169000,N'Ngôn ngữ Python','~/Content/Images/Python.png'),
('KH2023005',N'Khóa học Lập trình C#',N'Khóa học này giúp bạn có thể tự lập trình được những phương thức xử lý dữ liệu bằng ngôn ngữ C#','Kteam',179000,N'Ngôn ngữ C#','~/Content/Images/CSharp.png'),
('KH2023006',N'Khóa học Lập trình Web',N'Khóa học này giúp bạn có thể tự lập trình được một trang web theo yêu cầu','ThuVienLapTrinh',249000,N'Lập trình Web','~/Content/Images/LTW.png'),
('KH2023007',N'Khóa học Thiết kế Web',N'Khóa học này giúp bạn có thể tự thiết kế được một giao diện web theo yêu cầu','f8chanel',249000,N'Thiết kế Web','~/Content/Images/TKW.png'),
('KH2023008',N'Khóa học Lập trình HTML và CSS',N'Khóa học này giúp bạn tìm hiểu và sử dụng thành thạo ngôn ngữ HTML và CSS','gola-goclamweb',179000,N'Ngôn ngữ HTML và CSS','~/Content/Images/cs1.png'),
('KH2023009',N'Khóa học Lập trình JavaSript',N'Khóa học này giúp bạn tìm hiểu và sử dụng thành thạo ngôn ngữ JavaSript','f8chanel',149000,N'Ngôn ngữ JavaSript','~/Content/Images/cs2.png'),
('KH2023010',N'Khóa học Bootstrap',N'Khóa học này giúp bạn tìm hiểu và sử dụng Bootstrap','f8chanel',149000,N'Giao diện Bootstrap','~/Content/Images/Bootstrap.png')

set dateformat dmy
insert into BaiGiang (MaBG, TieuDeBG, NoiDungBG, MaKH, ThuTuBaiHoc, Video)
values 
--ngôn ngữ c 
('BG20230001',N'Bài Mở Đầu Về Lập Trình C',N'Kiểu Dữ Liệu Và Vào Ra Trong Ngôn Ngữ Lập Trình C','KH2023001',1,'https://www.youtube.com/watch?v=vpqMmfkSAMo&list=PLux-_phi0Rz2TB5D16sJzy3MgOht3IlND&ab_channel=28tech'),
('BG20230002',N'Cấu Trúc Rẽ Nhánh',N'IF ELSE Và Bảng Mã ASCII','KH2023001',2,'https://www.youtube.com/watch?v=4X8aXn0dMMM&list=PLux-_phi0Rz2TB5D16sJzy3MgOht3IlND&index=2&ab_channel=28tech'),
('BG20230003',N'Cấu Trúc Rẽ Nhánh',N'Switch Case và Câu Lệnh GOTO','KH2023001',3,'https://www.youtube.com/watch?v=MYtFJrmPgUg&list=PLux-_phi0Rz2TB5D16sJzy3MgOht3IlND&index=3&ab_channel=28tech'),
--ngôn ngữ c++
('BG20230004',N'Làm Quen Với Ngôn Ngữ Lập Trình C++',N'Vào Ra Trong C++| Kiểu Dữ Liệu Và Biến Trong C++','KH2023002',1,'https://www.youtube.com/watch?v=74B6PXO97Tw&list=PLux-_phi0Rz0Hq9fDP4TlOulBl8APKp79&index=2&ab_channel=28tech'),
('BG20230005',N'Làm Quen Với Ngôn Ngữ Lập Trình C++',N'Toàn Tập Về Các Toán Tử Cơ Bản Trong Ngôn Ngữ Lập Trình C++','KH2023002',2,'https://www.youtube.com/watch?v=74B6PXO97Tw&list=PLux-_phi0Rz0Hq9fDP4TlOulBl8APKp79&index=2&ab_channel=28tech'),
('BG20230006',N'Cấu Trúc Rẽ Nhánh Trong Ngôn Ngữ Lập Trình C++',N'IF ELSE và SWITCH CASE Trong C++','KH2023002',3,'https://www.youtube.com/watch?v=74B6PXO97Tw&list=PLux-_phi0Rz0Hq9fDP4TlOulBl8APKp79&index=2&ab_channel=28tech'),
--ngôn ngữ Java
('BG20230007',N'Khóa học lập trình Java đến OOP',N'Giới thiệu Java','KH2023003',1,'https://www.youtube.com/watch?v=3gtOAlcovoQ&list=PL33lvabfss1yGrOutFR03OZoqm91TSsvs&index=1&ab_channel=Kteam'),
('BG20230008',N'Khóa học lập trình Java đến OOP',N'Cài đặt môi trường Java','KH2023003',2,'https://www.youtube.com/watch?v=3gtOAlcovoQ&list=PL33lvabfss1yGrOutFR03OZoqm91TSsvs&index=1&ab_channel=Kteam'),
('BG20230009',N'Khóa học lập trình Java đến OOP',N'Chương trình Java đầu tiên','KH2023003',3,'https://www.youtube.com/watch?v=3gtOAlcovoQ&list=PL33lvabfss1yGrOutFR03OZoqm91TSsvs&index=1&ab_channel=Kteam'),
--ngôn ngữ Python
('BG20230010',N'Khóa học lập trình Python cơ bản',N'Khóa học lập trình Python cơ bản','KH2023004',1,'https://www.youtube.com/watch?v=NZj6LI5a9vc&list=PL33lvabfss1xczCv2BA0SaNJHu_VXsFtg&ab_channel=Kteam'),
('BG20230011',N'Khóa học lập trình Python cơ bản',N'Cài đặt môi trường Python','KH2023004',2,'https://www.youtube.com/watch?v=NZj6LI5a9vc&list=PL33lvabfss1xczCv2BA0SaNJHu_VXsFtg&ab_channel=Kteam'),
('BG20230012',N'Khóa học lập trình Python cơ bản',N'Chạy file Python','KH2023004',3,'https://www.youtube.com/watch?v=NZj6LI5a9vc&list=PL33lvabfss1xczCv2BA0SaNJHu_VXsFtg&ab_channel=Kteam'),
--ngôn ngữ c#
('BG20230013',N'Khóa học lập trình C# Cơ bản',N'Cấu trúc lệnh cơ bản','KH2023005',1,'https://www.youtube.com/watch?v=FhAIc0tlyaQ&list=PL33lvabfss1wUj15ea6W0A-TtDOrWWSRK&index=2&ab_channel=Kteam'),
('BG20230014',N'Khóa học lập trình C# Cơ bản',N'Nhập xuất cơ bản','KH2023005',2,'https://www.youtube.com/watch?v=FhAIc0tlyaQ&list=PL33lvabfss1wUj15ea6W0A-TtDOrWWSRK&index=2&ab_channel=Kteam'),
('BG20230015',N'Khóa học lập trình C# Cơ bản',N'Biến trong C# ','KH2023005',3,'https://www.youtube.com/watch?v=FhAIc0tlyaQ&list=PL33lvabfss1wUj15ea6W0A-TtDOrWWSRK&index=2&ab_channel=Kteam'),
--lập trình web
('BG20230016',N'Tự học ASP.NET MVC 5',N'Giới thiệu, tạo project trong Visual Studio, cấu hình IIS','KH2023006',1,'https://www.youtube.com/watch?v=9aQbJLd3A_U&list=PL2_xbcFZM80PHczxl-jlOgKA4yus3kWwu&index=74&ab_channel=Th%C6%B0Vi%E1%BB%87nL%E1%BA%ADpTr%C3%ACnhEDU'),
('BG20230017',N'Tự học ASP.NET MVC 5',N'Cấu hình URL thân thiện, sử dụng Layout trong thiết kế View','KH2023006',2,'https://www.youtube.com/watch?v=RqSuDOuc4CA&list=PL2_xbcFZM80PHczxl-jlOgKA4yus3kWwu&index=75&ab_channel=Th%C6%B0Vi%E1%BB%87nL%E1%BA%ADpTr%C3%ACnhEDU'),
('BG20230018',N'Tự học ASP.NET MVC 5',N'Làm việc với database bằng Entity Framework Code First','KH2023006',3,'https://www.youtube.com/watch?v=VJNUxjn3HS0&list=PL2_xbcFZM80PHczxl-jlOgKA4yus3kWwu&index=76&ab_channel=Th%C6%B0Vi%E1%BB%87nL%E1%BA%ADpTr%C3%ACnhEDU'),
--Thiết kế web
('BG20230019',N'Giao diện web cơ bản',N'Giới thiệu dự án The Band | Thực hành cắt HTML CSS cơ bản | Phân tích giao diện web
','KH2023007',1,'https://www.youtube.com/watch?v=RPHBgBsw6Xg&list=PL_-VfJajZj0U9nEXa4qyfB4U5ZIYCMPlz&index=83&ab_channel=F8Official'),
('BG20230020',N'Giao diện web cơ bản',N'Phân tích dự án','KH2023007',2,'https://www.youtube.com/watch?v=b8Z5Cyod9oI&list=PL_-VfJajZj0U9nEXa4qyfB4U5ZIYCMPlz&index=86'),
('BG20230021',N'Giao diện web cơ bản',N'Tạo khung dự án The Band | Dựng project base | Khóa học HTML, CSS cơ bản cho người mới bắt đầu','KH2023007',3,'https://www.youtube.com/watch?v=vBWCymyUySw&list=PL_-VfJajZj0U9nEXa4qyfB4U5ZIYCMPlz&index=85'),
--HTML,CSS
('BG20230022',N'Giới thiệu khóa học HTML-CSS',N'Các thẻ HTML cơ bản','KH2023008',1,'https://www.youtube.com/watch?v=B51n2ucFn_I&list=PLgk9x84DhO7X1cP-jI8UgtGmsJpXYdafe&index=2&ab_channel=Gola-G%C3%B3cl%C3%A0mweb'),
('BG20230023',N'Giới thiệu khóa học HTML-CSS',N'Các cách thêm định dạng css vào file HTML','KH2023008',2,'https://www.youtube.com/watch?v=zOQKvo-YD6k&list=PLgk9x84DhO7X1cP-jI8UgtGmsJpXYdafe&index=4&ab_channel=Gola-G%C3%B3cl%C3%A0mweb'),
('BG20230024',N'Giới thiệu khóa học HTML-CSS',N'Một số thuộc tính cơ bản của CSS','KH2023008',3,'https://www.youtube.com/watch?v=CRLl-csb2kw&list=PLgk9x84DhO7X1cP-jI8UgtGmsJpXYdafe&index=6&ab_channel=Gola-G%C3%B3cl%C3%A0mweb'),
--javasript
('BG20230025',N'Học lập trình Javascript cơ bản',N'Cài đặt môi trường','KH2023009',1,'https://www.youtube.com/watch?v=efI98nT8Ffo&list=PL_-VfJajZj0VgpFpEVFzS5Z-lkXtBe-x5&index=3&ab_channel=F8Official'),
('BG20230026',N'Học lập trình Javascript cơ bản',N'Sử dụng JS trong file HTML','KH2023009',2,'https://www.youtube.com/watch?v=W0vEUmyvthQ&list=PL_-VfJajZj0VgpFpEVFzS5Z-lkXtBe-x5&index=4&ab_channel=F8Official'),
('BG20230027',N'Học lập trình Javascript cơ bản',N'Khai báo biến','KH2023009',3,'https://www.youtube.com/watch?v=CLbx37dqYEI&list=PL_-VfJajZj0VgpFpEVFzS5Z-lkXtBe-x5&index=5&ab_channel=F8Official'),
--Bootstrap
('BG20230028',N'Cách sử dụng Bootstrap',N'Làm quen với bootstrap - container - grid system - column - row - card','KH2023010',1,'https://www.youtube.com/watch?v=UvYPMtghT3s&list=PLieAxB9_noZlT2x2jt8HgY36y8fDqeE3X&ab_channel=Th%E1%BA%A7yH%E1%BB%99Fpoly'),
('BG20230029',N'Xây dựng Bootstrap',N'Xây dựng layout website với bootstrap','KH2023010',2,'https://www.youtube.com/watch?v=9ihpTwKxzbQ&list=PLieAxB9_noZlT2x2jt8HgY36y8fDqeE3X&index=2&ab_channel=Th%E1%BA%A7yH%E1%BB%99Fpoly'),
('BG20230030',N'Xây dựng Layout Website với Bootstrap',N'Copy màu, bố sung css','KH2023010',3,'https://www.youtube.com/watch?v=6n-vUW34S9w&list=PLieAxB9_noZlT2x2jt8HgY36y8fDqeE3X&index=4&ab_channel=Th%E1%BA%A7yH%E1%BB%99Fpoly')

SET DATEFORMAT DMY
INSERT INTO BaiTap
VALUES
--C--
('BT2023001',N'Bài Tập Mở Đầu Về Lập Trình C',N'Dãy số Fibonacci, Check Số nguyên tố, Tính giai thừa và Chuyển đổi hệ cơ số','15/11/2022','KH2023001'),
('BT2023002',N'Bài Tập Về Cấu Trúc Rẽ Nhánh IF ELSE Và Bảng Mã ASCII',N'Tìm số ngày của năm N, biết rằng năm nhuận là năm chia hết cho 400 hoặc chia hết cho 4 nhưng không chia hết cho 100. Ví dụ, các năm 2000, 2004 là năm nhuận và có số ngày là 366, các năm 1900, 1945 không phải là năm nhuận và có số ngày là 365.','30/11/2022','KH2023001'),
('BT2023003',N'Bài Tập Về Cấu Trúc Rẽ Nhánh Switch Case',N'Nhập vào 2 số nguyên a, b và nhập vào phép toán +, -, *, / Thực hiện tính toán theo phép toán nhập vào với hai số a, b','15/12/2022','KH2023001'),
--C++--
('BT2023004',N'Làm Quen Với Ngôn Ngữ Lập Trình C++',N'Dãy số Fibonacci, Check Số nguyên tố, Tính giai thừa và Chuyển đổi hệ cơ số','15/11/2022','KH2023002'),
('BT2023005',N'Bài Tập Về Cấu Trúc Rẽ Nhánh IF ELSE Và Bảng Mã ASCII',N'Tìm số ngày của năm N, biết rằng năm nhuận là năm chia hết cho 400 hoặc chia hết cho 4 nhưng không chia hết cho 100. Ví dụ, các năm 2000, 2004 là năm nhuận và có số ngày là 366, các năm 1900, 1945 không phải là năm nhuận và có số ngày là 365.','30/11/2022','KH2023002'),
('BT2023006',N'Bài Tập Về Cấu Trúc Rẽ Nhánh Switch Case ',N'Nhập vào 2 số nguyên a, b và nhập vào phép toán +, -, *, / Thực hiện tính toán theo phép toán nhập vào với hai số a, b','15/12/2022','KH2023002'),
--Java--
('BT2023007',N'Bài Tập Cơ Bản Về Java',N'Hãy viết chương trình tính tổng các chữ số của một số nguyên bất kỳ.','15/11/2022','KH2023003'),
('BT2023008',N'Bài Tập Về Chuỗi',N'Sắp xếp chuỗi theo thứ tự bảng chữ cái','30/11/2022','KH2023003'),
('BT2023009',N'Bài Tập Về Vòng Lặp',N'Cách in các phần tử trùng nhau trong mảng Java','15/12/2022','KH2023003'),
--Python---
('BT2023013',N'Làm Quen Với Ngôn Ngữ Lập Trình Python',N'Dãy số Fibonacci, Check Số nguyên tố, Tính giai thừa và Chuyển đổi hệ cơ số','15/11/2022','KH2023004'),
('BT2023014',N'Bài Tập Về Cấu Trúc Rẽ Nhánh IF ELSE Và Bảng Mã ASCII',N'Tìm số ngày của năm N, biết rằng năm nhuận là năm chia hết cho 400 hoặc chia hết cho 4 nhưng không chia hết cho 100. Ví dụ, các năm 2000, 2004 là năm nhuận và có số ngày là 366, các năm 1900, 1945 không phải là năm nhuận và có số ngày là 365.','30/11/2022','KH2023004'),
('BT2023015',N'Bài Tập Về Cấu Trúc Rẽ Nhánh Switch Case',N'Nhập vào 2 số nguyên a, b và nhập vào phép toán +, -, *, / Thực hiện tính toán theo phép toán nhập vào với hai số a, b','15/12/2022','KH2023004'),
--C#--
('BT2023010',N'Bài Tập Cơ Bản Lập trình C#',N'Hãy viết chương trình tính tổng các chữ số của một số nguyên bất kỳ.','15/11/2022','KH2023005'),
('BT2023011',N'Bài Tập Về Chuỗi',N'Sắp xếp chuỗi theo thứ tự bảng chữ cái','30/11/2022','KH2023005'),
('BT2023012',N'Bài Tập Về Vòng Lặp',N'Cách in các phần tử trùng nhau trong mảng C#','15/12/2022','KH2023005'),
--LTW--
('BT2023019',N'Bài Tập Đầu Khóa',N'Tạo project trong Visual Studio, cấu hình IIS','15/11/2022','KH2023006'),
('BT2023020',N'Bài Tập Giữa Khóa',N'Sử dụng Layout trong thiết kế View','30/11/2022','KH2023006'),
('BT2023021',N'Bài Tập Cuối Khóa',N'Làm việc với database bằng Entity Framework Code First','15/12/2022','KH2023006'),
--Thiết kế web--
('BT2023016',N'Bài Tập Cơ Bản Thiết kế Web',N'Design đổi màu chữ, phông chữ và đổi nền cho chữ:"HELLO WORLD"','15/11/2022','KH2023007'),
('BT2023017',N'Bài Tập Về HTML và CSS',N'Hãy Design một danh sách tùy ý theo dạng bảng','30/11/2022','KH2023007'),
('BT2023018',N'Bài Tập Về JavaSript',N'Hãy ràng buộc lọc sản phẩm bằng JavaSript','15/12/2022','KH2023007'),

--HTML--
('BT2023022',N'Bài Tập Cơ Bản HTML',N'Sự dụng các thẻ HTML cơ bản','15/11/2022','KH2023008'),
('BT2023023',N'Bài Tập Làm Quen Với CSS',N'Các cách thêm định dạng css vào file HTML','30/11/2022','KH2023008'),
('BT2023024',N'Bài Tập về CSS',N'Thực hành với một số thuộc tính cơ bản của CSS','15/12/2022','KH2023008'),
--JavaSript--
('BT2023025',N'Bài Tập Giới Thiệu JavaSript',N'Cài đặt môi trường  JavaSript','15/11/2022','KH2023009'),
('BT2023026',N'Bài Tập Cơ Bản JavaSript',N'Cách sử dụng JS trong file HTML','30/11/2022','KH2023009'),
('BT2023027',N'Bài Tập Cuối Khóa',N'Ràng buộc JavaSript trong đăng ký, đăng nhập','15/12/2022','KH2023009'),
--Bootstrap--
('BT2023028',N'Bài Tập Cơ Bản Bootstrap',N'Làm quen với bootstrap - container - grid system - column - row - card','15/11/2022','KH2023010'),
('BT2023029',N'Bài Tập Xây dựng layout website với Bootstrap',N'Xây dựng layout website với bootstrap','30/11/2022','KH2023010'),
('BT2023030',N'Bài Tập Cuối Khóa',N'Xây dựng trang bán sản phẩm bằng cách sử dụng Bootstrap','15/12/2022','KH2023010')

INSERT INTO KetQua
VALUES
('KQ20230001','ND20230001','BT2023001',4.5),
('KQ20230002','ND20230001','BT2023002',6.7),
('KQ20230003','ND20230001','BT2023003',5)











-- DTB >= 8 là Giỏi, DTB >= 6.5 và < 8 là Khá, DTB >= 5 và < 6.5 là Trung Bình, DTB >= 4 và < 5 là Yếu, DTB < 4 là Kém
set dateformat dmy


select * from KhoaHoc
select * from nguoidung
select * from HoaDonDky
select * from ChiTietHoaDon
select * from BaiGiang 
select * from CourseCart

--store procedure
--search
--GO
--CREATE PROCEDURE SearchCourses
--    @searchStr NVARCHAR(100)
--AS
--BEGIN
--    SET @searchStr = '%' + @searchStr + '%'
--    SELECT * 
--    FROM KhoaHoc
--    WHERE TenMonHoc LIKE @searchStr
--END

--exec SearchCourses @searchStr = 'C++'
