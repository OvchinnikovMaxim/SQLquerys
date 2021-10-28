using System;

namespace SQLquerys
{
	/// <summary>
	/// Запросы в базы на сервере 2.7
	/// </summary>
	public class Query
	{
		/// <summary>
		/// логин и пароль сотрудника для нового сотрудника в nc_soft, direditor и прикрепление к заводу - база nefco
		/// </summary>
		/// <param name="login"></param>
		/// <param name="fio"></param>
		/// <param name="pass"></param>
		/// <param name="zavod"></param>
		/// <returns></returns>
		public string user_inf_new(string login, string fio, string pass, int zavod)
		{
			string query =@"INSERT INTO nefco.dbo.user_inf (user_id,status_id,user_name,region_id,pass,flag) " +
				"VALUES ('"+login+"',1,'"+fio+"',0,'"+pass+"',0);" +
				"DECLARE @id INT;" +
				"SET @id = (SELECT @@IDENTITY);" +
				"INSERT INTO nefco.dbo.auth_factory (user_id,factory_id) VALUES (@id,"+zavod+")";
			
			return query;
		}
		
		/// <summary>
		/// логин и пароль сотрудника для  nc_soft, direditor и прикрепление к заводу - база nefco
		/// </summary>
		/// <param name="surname">Фамилия</param>
		/// <returns>Строка запроса</returns>
		public string User_inf(string surname)
		{
			string query = @"SELECT ui.id, ui.user_id, ui.pass, ui.user_name, ui.date_end, af.factory_id, 
							CASE af.factory_id 
								WHEN  1 THEN 'НК' 
								WHEN  2 THEN 'НБП' 
								ELSE '' 
							END AS 'завод' 
							FROM nefco.dbo.user_inf ui LEFT JOIN 
							nefco.dbo.auth_factory af ON ui.id = af.user_id 
							WHERE  ui.user_name  LIKE '%"+surname+"%' ORDER BY 4";
			
			return query;
		}
		
		/// <summary>
		/// какие программы у пользователя nc_soft, direditor и т.д. - база nefco
		/// </summary>
		/// <param name="userID">Идентификатор сотрудника для nc_soft, direditor</param>
		/// <returns>Строка запроса</returns>
		public string user_inf_profile(int userID)
		{
			string query =@"SELECT u.id, u.user_id, u.user_name, af.factory_id, ap.prgm_id, p.prgm_name, aup.prf_id, ap.prf_name 
							FROM nefco.dbo.user_inf u
							LEFT JOIN nefco.dbo.auth_factory af ON u.id = af.user_id
							LEFT JOIN nefco.dbo.auth_usr_prf aup ON u.user_id = aup.usr_id
							LEFT JOIN nefco.dbo.auth_prf ap ON aup.prf_id = ap.prf_id
							LEFT JOIN nefco.dbo.auth_prgm p ON ap.prgm_id = p.prgm_id
							LEFT JOIN nefco.dbo.auth_prgm_tags pt ON pt.prgm_id=p.prgm_id 
							LEFT JOIN nefco.dbo.auth_prf_tags apt ON ap.prf_id = apt.prf_id AND pt.tag=apt.tag 
							WHERE u.id IN ("+userID+") GROUP BY u.id, u.user_id, u.user_name, af.factory_id, ap.prgm_id, p.prgm_name, aup.prf_id, ap.prf_name ORDER BY prgm_id, user_name,prgm_name, prf_name ";
			
			return query;
		}
		
		/// <summary>
		/// данные по сотруднику для сайта ncsd.ru - база nefco
		/// </summary>
		/// <param name="login">логин</param>
		/// <param name="name">имя</param>
		/// <param name="surname">фамилия</param>
		/// <param name="userID">код сотрудника</param>
		/// <returns>Строка запроса</returns>
		public string NCSD(string login, string name, string surname, int userID)
		{
			string query=@"SELECT 
							cu.id AS 'ИД сотрудника для ncsd',
							cu.login AS 'Логин',
							cu.password AS 'Пароль',
							cu.del AS 'Пометка на удаление',	
							cp.id AS 'ИД физического лица',
							cp.name AS 'Имя',
							cp.surname AS 'Фамилия',
							cp.patronymic AS 'Отчество',
							cp.is_candidate AS 'Кандидат',
							CASE cp.factory_id
								WHEN  1 THEN 'НК'
								WHEN  2 THEN 'НБП'
								ELSE ''
							END AS 'завод',	
							e.id AS 'Код сотрудника',
							e.date_end AS 'Дата окончания работы',
							e.staff_id AS 'Код ставки - staff_id',
							e.del AS 'Удаленный сотрудник'
						FROM nefco.dbo.co_user cu
						  LEFT JOIN nefco.dbo.co_person cp ON cu.id=cp.user_id
						  LEFT JOIN nefco.dbo.employee e ON e.person_id=cp.id
						  WHERE
						   cu.login LIKE '%"+login+
						   "%' OR cp.name LIKE '"+name+
						   "' OR cp.surname LIKE '%"+surname+
						   "%' OR e.id="+userID;
			return query;
		}
		
		/// <summary>
		/// информация по дистрибьютору - база nefco
		/// </summary>
		/// <param name="name">наименование дистра</param>
		/// <param name="contractor">код дистра</param>
		/// <returns>Строка запроса</returns>
		public string Distr(string name, int contractor)
		{
			string query =@"DECLARE @distr VARCHAR(50) = '%"+name+
				"%' DECLARE @contactor_id_distr INT ="+contractor+";" +
				"SELECT d.*, ccd.fio, ccd.password, ccd.login, ccd.isMT, ccd.is_mt_sales, ccd.is_mt_ost " +
				"FROM nefco.dbo.distr d LEFT JOIN nefco.dbo.client_card_distrpass ccd on d.distr_id=ccd.distr_id " +
				"WHERE d.distr LIKE @distr OR d.contractor_id IN(@contactor_id_distr)";
			return query;
		}
		
		/// <summary>
		/// логины для дистров - база nefco
		/// </summary>
		/// <param name="zavod">код завода</param>
		/// <returns>Строка запроса</returns>
		public string login_distr(int zavod)
		{
			string query="";
			switch (zavod) 
			{
				case 1:
					query = @"SELECT * FROM nefco.dbo.client_card_distrpass 
											WHERE login LIKE 'distr%'
														ORDER BY login DESC";
					break;
				case 2:
					query = @"SELECT * FROM nefco.dbo.client_card_distrpass 
											WHERE login LIKE 'kdistr%'
														ORDER BY login DESC";
					break;
				default:
					query = "Номер завода не указан или указан не верно";
					break;
			}
			return query;
		}
		
		/// <summary>
		/// заведение нового дистрибьютора в базе nefco
		/// </summary>
		/// <param name="contractor">код дистра</param>
		/// <param name="zavod">код завода 1-НК, 2- КЖК, НБП</param>
		/// <param name="pass">пароль - 8 символов</param>
		/// <param name="login">логин - сначала проверить занятые логины для дистров</param>
		/// <param name="isMT">будет ли мобильная торговля 0 -нет, 1- да</param>
		/// <param name="isMI">будет ли модуль интеграции 0 -нет, 1- да</param>
		/// <returns>Строка запроса</returns>
		public string new_distr(int contractor,int zavod,string pass,string login, int isMT, int isMI)
		{
			// ставка оператора дистра
			int staff_list_id=0;
			
			int isMI_ost=0;
			
			#region присвоение staff_list_id и isMI_ost
			if (zavod==1) 
			{
				staff_list_id=641;
			} 
			else 
			{
				if (zavod==2) 
				{
					staff_list_id=726;
				}
			}
			
			if (isMI==1) 
			{
				isMI_ost=1;
			} 
			else 
			{
				isMI_ost = 0;
			}
			#endregion
			
			string query = @"set transaction isolation level read uncommitted;"+
				"declare @distr_id_const varchar (60)= (select distr_id from nefco.dbo.co_contractor_attr_customer where contractor_id="+contractor+"); "+
				"declare @contactor_id_const int = "+contractor+"; " +
				"declare @factory_id_const int = (select factory_id from nefco.dbo.co_contractor where id = @contactor_id_const); "+
				"declare @limit_const int = 15; " +
				"declare @staff_list_id_const int = "+staff_list_id+"; " +
				"INSERT INTO nefco.dbo.client_card_distrpass ( distr_id,fio,dostup,password,login,email,isMT,is_mt_sales, is_mt_ost) " +
				"VALUES (@distr_id_const,(select name from nefco.dbo.co_contractor where id = @contactor_id_const),'admin','"+pass+"','"+login+"','"+login+"',"+isMT+","+isMI+", '"+isMI_ost+"'); "+
@"declare @person_id int, @user_id int,@emp_id int; 

insert into nefco.dbo.co_user (login,password,del)
 values (SUBSTRING('"+login+"',1,60),SUBSTRING(master.dbo.fn_varbintohexstr(HashBytes('MD5','"+pass+"'+'ole98KHi79(*&^98jdsjhjh9303jhKGGF&^%&^t34kkjhsad8(*&gFE%$454')), 3, 32),0);"+

@"set @user_id = (SELECT IDENT_CURRENT('nefco.dbo.[co_user]'));
        
insert into nefco.dbo.co_person (surname,[name],patronymic,email, [user_id], factory_id)
 values (SUBSTRING((select name from nefco.dbo.co_contractor where id = @contactor_id_const),1,30), 'Оператор', SUBSTRING(@distr_id_const,1,30), '"+login+"', @user_id, @factory_id_const);"+

				@"set @person_id=(SELECT IDENT_CURRENT('nefco.dbo.[co_person]'));
        
insert into nefco.dbo.employee(person_id,staff_list_id,date_begin,del)
 values (@person_id,@staff_list_id_const,GETDATE(),0);

set @emp_id=(SELECT IDENT_CURRENT('nefco.dbo.[employee]'));

insert into nefco.dbo.employee_contractor (employee_id,[contractor_id],main)
 values (@emp_id,@contactor_id_const,1);

insert into  nefco.dbo.distr_limit_sales  (distr_id, limit)
 VALUES (@distr_id_const,@limit_const)";
			return query;
		}
		
		/// <summary>
		/// продажи в unity, у которых fdel=0 - база unity
		/// </summary>
		/// <param name="contractor">код дистра</param>
		/// <param name="begin">дата начала периода</param>
		/// <param name="end">дата конца периода</param>
		/// <returns>Строка запроса</returns>
		public string select_sales_Rub_UNITY(int contractor, DateTime begin, DateTime end)
		{
			string query = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " +
				"DECLARE @contactor_id INT = "+contractor+"; " +
				"DECLARE @vdate1 SMALLDATETIME = '"+begin.ToShortDateString()+"'; " +
				"DECLARE @vdate2 SMALLDATETIME = '"+end.ToShortDateString()+"'; " +
				"DECLARE @FDEL INT = 0; " +
				"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " +
				"SELECT SUM(vsum_rub) AS unity FROM unity.dbo.mt_details_db " +
				"WHERE rdoc_id in (" +
					"SELECT md.DOC_ID FROM unity.dbo.MT_DOCS_db md " +
					"WHERE md.contractor_id = @contactor_id AND md.VDATE BETWEEN @vdate1 AND @vdate2 AND md.rdoc_type IN (2,4,5) AND md.FDEL=@FDEL AND md.RSTATE=1" +
				") AND FDEL=@FDEL";
			
			return query;
		}
		
		/// <summary>
		/// продажи в nefco, у которых fdel=0 - база nefco
		/// </summary>
		/// <param name="contractor">код дистра</param>
		/// <param name="begin">дата начала периода</param>
		/// <param name="end">дата конца периода</param>
		/// <returns>Строка запроса</returns>
		public string select_sales_Rub_NEFCO(int contractor, DateTime begin, DateTime end)
		{
			string query = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " +
				"DECLARE @contactor_id INT = "+contractor+"; " +
				"DECLARE @vdate1 SMALLDATETIME = '"+begin.ToShortDateString()+"'; " +
				"DECLARE @vdate2 SMALLDATETIME = '"+end.ToShortDateString()+"'; " +
				"DECLARE @FDEL INT = 0; " +
				"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " +
				"SELECT SUM(vsum_rub) AS nefco FROM nefco.dbo.mt_details_db " +
				"WHERE rdoc_id in (" +
					"SELECT md.DOC_ID FROM nefco.dbo.MT_DOCS_db md " +
					"JOIN nefco.dbo.co_contractor cc on md.contractor_id=cc.id " +
					"WHERE contractor_id = @contactor_id AND md.VDATE BETWEEN @vdate1 AND @vdate2 AND md.rdoc_type IN (2,4,5) AND md.FDEL=@FDEL AND md.RSTATE=1" +
				") AND FDEL=@FDEL";
			
			return query;
		}
		
		/// <summary>
		/// Пометка табличной части накладной на удаление в базе nefco
		/// </summary>
		/// <param name="contractor">код дистра</param>
		/// <param name="begin">дата начала периода</param>
		/// <param name="end">дата конца периода</param>
		/// <returns>Строка запроса</returns>
		public string updateDETAILS_nefco(int contractor, DateTime begin, DateTime end)
		{
			string update_details = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " +
				"DECLARE @contactor_id INT = "+contractor+"; " +
				"DECLARE @vdate1 SMALLDATETIME = '"+begin.ToShortDateString()+"'; " +
				"DECLARE @vdate2 SMALLDATETIME = '"+end.ToShortDateString()+"'; " +
				"DECLARE @FDEL INT = 0; " +
				"UPDATE  nefco.dbo.mt_details_db SET FDEL=1 WHERE rdoc_id IN (SELECT DOC_ID FROM  nefco.dbo.MT_DOCS_db  WHERE contractor_id =  @contactor_id AND VDATE  BETWEEN @vdate1 AND @vdate2 AND rdoc_type IN (2,4,5) AND FDEL=@FDEL) AND FDEL=@FDEL";
			
			return update_details;
		}
		
		/// <summary>
		/// Пометка табличной части накладной на удаление в базе unity
		/// </summary>
		/// <param name="contractor">код дистра</param>
		/// <param name="begin">дата начала периода</param>
		/// <param name="end">дата конца периода</param>
		/// <returns>Строка запроса</returns>
		public string updateDETAILS_unity(int contractor, DateTime begin, DateTime end)
		{
			string update_details = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " +
				"DECLARE @contactor_id INT = "+contractor+"; " +
				"DECLARE @vdate1 SMALLDATETIME = '"+begin.ToShortDateString()+"'; " +
				"DECLARE @vdate2 SMALLDATETIME = '"+end.ToShortDateString()+"'; " +
				"DECLARE @FDEL INT = 0; " +
				"UPDATE  unity.dbo.mt_details_db SET FDEL=1 WHERE rdoc_id IN (SELECT DOC_ID FROM  unity.dbo.MT_DOCS_db  WHERE contractor_id =  @contactor_id AND VDATE  BETWEEN @vdate1 AND @vdate2 AND rdoc_type IN (2,4,5) AND FDEL=@FDEL) AND FDEL=@FDEL";
			
			return update_details;
		}
		
		/// <summary>
		/// Пометка накладных на удаление в базе nefco
		/// </summary>
		/// <param name="contractor">код дистра</param>
		/// <param name="begin">дата начала периода</param>
		/// <param name="end">дата конца периода</param>
		/// <returns>Строка запроса</returns>
		public string updateDOCS_nefco(int contractor, DateTime begin, DateTime end)
		{
			string update_docs = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " +
				"DECLARE @contactor_id INT = "+contractor+"; " +
				"DECLARE @vdate1 SMALLDATETIME = '"+begin.ToShortDateString()+"'; " +
				"DECLARE @vdate2 SMALLDATETIME = '"+end.ToShortDateString()+"'; " +
				"DECLARE @FDEL INT = 0; " +
				"UPDATE nefco.dbo.MT_DOCS_db SET FDEL=1 WHERE contractor_id =  @contactor_id AND VDATE  BETWEEN @vdate1 AND @vdate2 AND rdoc_type IN (2,4,5) AND FDEL=@FDEL";
			
			return update_docs;
		}
		
		/// <summary>
		/// Пометка накладных на удаление в базе unity
		/// </summary>
		/// <param name="contractor">код дистра</param>
		/// <param name="begin">дата начала периода</param>
		/// <param name="end">дата конца периода</param>
		/// <returns>Строка запроса</returns>
		public string updateDOCS_unity(int contractor, DateTime begin, DateTime end)
		{
			string update_docs = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " +
				"DECLARE @contactor_id INT = "+contractor+"; " +
				"DECLARE @vdate1 SMALLDATETIME = '"+begin.ToShortDateString()+"'; " +
				"DECLARE @vdate2 SMALLDATETIME = '"+end.ToShortDateString()+"'; " +
				"DECLARE @FDEL INT = 0; " +
				"UPDATE unity.dbo.MT_DOCS_db SET FDEL=1 WHERE contractor_id =  @contactor_id AND VDATE  BETWEEN @vdate1 AND @vdate2 AND rdoc_type IN (2,4,5) AND FDEL=@FDEL";
			
			return update_docs;
		}
		
		/// <summary>
		/// Актиализация накладных для обработки в базе unity
		/// </summary>
		/// <param name="contractor">код дистра</param>
		/// <param name="begin">дата начала периода</param>
		/// <param name="end">дата конца периода</param>
		/// <returns>Строка запроса</returns>
		public string actualDOCS_unity(int contractor, DateTime begin, DateTime end)
		{
			string query ="SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; update unity.dbo.MT_DOCS_db set updated=1 where contractor_id = "+contractor+" AND VDATE  BETWEEN '"+begin.ToShortDateString()+"' AND '"+end.ToShortDateString()+"' AND rdoc_type IN (2,4,5) AND FDEL=0";
			
			return query;
		}
		
		/// <summary>
		/// Запуск обработки продаж из unity в nefco - база unity
		/// </summary>
		/// <param name="contractor">код дистра</param>
		/// <param name="begin">дата начала периода</param>
		/// <param name="end">дата конца периода</param>
		/// <returns>Строка запроса</returns>
		public string TRANSFER_MT_DOCS_UNITY(int contractor, DateTime begin, DateTime end)
		{
			string query ="SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; EXEC unity.dbo.TRANSFER_MT_DOCS_UNITY '"+begin.ToShortDateString()+"', '"+end.ToShortDateString()+"', "+contractor;
			
			return query;
		}
		
		/// <summary>
		/// продукция в последнем опубликованном неудаленном прайс-листе - база nefco
		/// </summary>
		/// <param name="contractor">код дистра</param>
		/// <returns>Строка запроса</returns>
		public string prod_in_last_price(int contractor)
		{
			string query =@"SELECT * FROM nefco.dbo.price_body WHERE price_id = (
								SELECT top 1 price_id FROM nefco.dbo.price_head WHERE type_id in (
									SELECT client_pricelist FROM nefco.dbo.co_contractor_attr_customer WHERE contractor_id in("+contractor+")) and pblsh=1 and del=0  ORDER BY date DESC)";
			
			return query;
		}
		
		/// <summary>
		/// список опубликованных и неудаленных прайсов дистра - база nefco
		/// </summary>
		/// <param name="contractor">код дистра</param>
		/// <returns>Строка запроса</returns>
		public string distr_prices(int contractor)
		{
			string query =@"SELECT * FROM nefco.dbo.price_head WHERE type_id in (
									SELECT client_pricelist FROM nefco.dbo.co_contractor_attr_customer WHERE contractor_id in("+contractor+")) and pblsh=1 and del=0  ORDER BY date DESC";
			
			return query;
		}
		
		/// <summary>
		/// список продукции в указанном прайс-листе - база nefco
		/// </summary>
		/// <param name="priceID">номер прайс-листа</param>
		/// <returns>Строка запроса</returns>
		public string prod_in_price(int priceID)
		{
			string query = @"SELECT * FROM nefco.dbo.price_body WHERE price_id = "+priceID+" ORDER BY material_id";
			
			return query;
		}
		
		/// <summary>
		/// Копирование ТТ на другого дистрибьютора, с сектора на сектор
		/// </summary>
		/// <param name="sectorFrom">номер сектора откуда копировать</param>
		/// <param name="sectorTo">номер сектора куда копировать</param>
		/// <param name="contractorFrom">код дистрибьютора откуда копировать</param>
		/// <param name="contractorTo">код дистрибьютора куда копировать</param>
		/// <returns>Строка запроса</returns>
		public string copyTT_distr_sector(int sectorFrom, int sectorTo, int contractorFrom, int contractorTo)
		{
			
			string query=@" declare @client1 int;
							declare @client2 int;
							DECLARE @count INT = 0; 
							declare @similar int;
							declare @contractorFrom int, @contractorTo int;
							
							declare @sectorFrom int = "+sectorFrom+"; "+
							"declare @sectorTo int = "+sectorTo+"; "+
							
							"set @contractorFrom="+contractorFrom+"; "+
							"set @contractorTo="+contractorTo+"; "+
							
							@"declare @employeeTo int = '1317';
							declare @distrTo varchar(10);
							set @distrTo = (select distr_id from nefco.dbo.co_contractor_attr_customer where contractor_id=@contractorTo)
							if ISNULL(@contractorTo, 0) = 0 BEGIN
								PRINT 'contractor_id not found';
								RETURN;
							END;
							
							
							declare @id int;
							set @id = (select max(id1) from nefco.dbo.client_card where contractor_id=@contractorTo)
							
							-- Сотрудник, закрепленный за сектором @sectorTo     
							 select top 1 @employeeTo=e.id
							from nefco.dbo.sam_sectors ss
							inner join nefco.dbo.co_staff_sector css on css.sector_id=ss.id 
							inner join nefco.dbo.employee_staff es on es.id=css.staff_id
							inner join nefco.dbo.employee e on e.staff_id=es.id --and e1.date_begin<=@date_end 
							     and e.date_end is null and e.del=0
							where ss.id=@sectorTo
							
							declare c cursor for
							SELECT c1.client_id,c1.similar_id
							FROM nefco.dbo.[client_card] c1
							WHERE c1.visible=0 and c1.contractor_id=@contractorFrom and c1.sector_id = @sectorFrom 
							--and c1.tnet_id=508 
							
							open c;
							fetch next from c into @client1,@similar;
							
							while @@fetch_status = 0 begin
							 IF @similar IS NULL 
							    begin
							      SET @similar=@client1;
							      update nefco.dbo.client_card set similar_id=@client1 where client_id=@client1;
							    end
							    set @id=@id+1   
							
							
							 INSERT INTO nefco.dbo.[client_card] ([INN], [client_name],
							   [client_adress], [urname], [uradress],  [parent_id],  
							   [type_id], [category_id], [square_area], 
							   [dostup_id], [oplata_id], [oplata_prolong], 
							  [distr_id], [id1], [city_id], [raion_id],
							[emp_id], [date_create], [visible], [fupd], 
							[tnet_id], [area_id], [resp_id], [sector_id], 
							   [factory_id], [postindex], [cspace], [visittime], [ordertype], [comment], [visitweek], 
							   [visitday], [new_card], [visible_2122010], [route_pos], [face_sms], 
							   [face_gms], [face_tm], [face_butter], [net_type_id], [similar_id], [city_kladr_id], 
							   [child_sales], [work_time_from], [work_time_to], [visit_time_from], [visit_time_to], 
							   [rest_time_from], [rest_time_to], [work_allday], [contact_id], [street_kladr_id], [house_num], [group_id], contractor_id) 
							 SELECT  [INN], [client_name], [client_adress], [urname], [uradress], 
							   NULL,  [type_id], [category_id], [square_area], 
							  [dostup_id],
							  [oplata_id], [oplata_prolong], @distrTo, @id, [city_id], 
							  [raion_id],
							@employeeTo, [date_create], [visible], 1,
							[tnet_id], [area_id], [resp_id], @sectorTo, [factory_id], [postindex], [cspace], 
							  [visittime], [ordertype], [comment], [visitweek], [visitday], [new_card], [visible_2122010], 
							[route_pos], [face_sms], [face_gms], [face_tm], [face_butter], [net_type_id], 
							  @similar, [city_kladr_id], [child_sales], [work_time_from], [work_time_to], [visit_time_from], 
							  [visit_time_to], [rest_time_from], [rest_time_to], [work_allday], [contact_id], [street_kladr_id], 
							  [house_num], [group_id], @contractorTo FROM nefco.dbo.client_card WHERE client_id=@client1;
							    
							    select @client2=(select client_id from nefco.dbo.client_card where id=(select top 1 @@identity from nefco.dbo.client_card))
							 
							 
							 insert into nefco.dbo.client_card_info ([client_id], [_shop_type_id], [location_id], [square_id], [service_format_id], [cash_desk_number], [shelves_size], [shelves_count], [margin_id], [warehouse], [sku_count_id])
							 select @client2, [_shop_type_id], [location_id], [square_id], [service_format_id], [cash_desk_number], [shelves_size], [shelves_count], [margin_id], [warehouse], [sku_count_id] 
							 from nefco.dbo.client_card_info where client_id = @client1
							
							 insert into nefco.dbo.client_card_info_mass ([client_id], [handbook_id])
							 select @client2, [handbook_id] from nefco.dbo.client_card_info_mass where client_id = @client1
							
							 insert into nefco.dbo.client_card_info_shelves ([client_id], [category_id], [value])
							 select @client2, [category_id], [value] from nefco.dbo.client_card_info_shelves where client_id = @client1
							
							 insert into nefco.dbo.client_card_info_representation ([client_id], [category_id], [segment_id], [value])
							 select @client2, [category_id], [segment_id], [value] from nefco.dbo.client_card_info_representation where client_id = @client1
							
							 -- копирования последней координаты клиента
							  insert into [nefco].[dbo].[gps_clients] ([tp_id], [utc], [local_time], [lat], [lng],  [not_utc], client_id )
							 select top 1 [tp_id], [utc], [local_time], [lat], [lng],  [not_utc], @client2 
							 from [nefco].[dbo].[gps_clients] where client_id = @client1
							 order by local_time desc
							
							 SET @count = @count + 1;
							 fetch next from c into @client1,@similar;
							end
							
							close c;
							deallocate c;
							PRINT @count;";
			
			return query;
		}
		
		/// <summary>
		/// Удаление заказа с записью в историю
		/// </summary>
		/// <param name="number">код заказа</param>
		/// <param name="comment">комментарий</param>
		/// <returns>Строка запроса</returns>
		public string del_zakaz_hat(int number, string comment)
		{
			string query =@"declare @order_id int;
							SET @order_id = "+number+@"; 
							
							INSERT INTO nefco.dbo.zakaz_hat_delete(id,num,date_imp,date_create,date_transp,[date],st,ver_id,zaivka_id,transp_id,saleman_id,
							                             distr_name,adress_d,skid,pay_id,sign,stand_zaivka,is_garmach,time_dos,itog_otchet,
							                             is_masspoddon,is_opened1111,is_notfinish,import_number,logistics_1_prc,nulled_passed,
							                             outage_passed,action_report,cc_signature,description,dscnt_3_mnplt,dscnt_5_mnplt,
							                             consignee_id,dscnt_mnplt,price_id,date_otgr,exp_kgk,err_id,dscnt_maket,is_mnplt_1prc,
							                             date_init,poddon_id,warehouse_id,buyer_num,buyer_date,time_transp,cost_ecod_state_id,
							                             dscnt_disposal,disposal_on,bonus_free,sum_from_1c,address_id,agreement_new,is_logistics,
							                             ecod_confirmation,[type],delivery_type_ecod,transp_types_ecod,delivery_instruction_ecod,
							                             location_name_ecod,locat_str_number_ecod,location_city_ecod,final_buyer_ecod,order_ext,
							                             prioritet)
							
							                              SELECT id,num,date_imp,date_create,date_transp,[date],st,ver_id,zaivka_id,transp_id,saleman_id,
							                             distr_name,adress_d,skid,pay_id,sign,stand_zaivka,is_garmach,time_dos,itog_otchet,
							                             is_masspoddon,is_opened1111,is_notfinish,import_number,logistics_1_prc,nulled_passed,
							                             outage_passed,action_report,cc_signature,description,dscnt_3_mnplt,dscnt_5_mnplt,
							                             consignee_id,dscnt_mnplt,price_id,date_otgr,exp_kgk,err_id,dscnt_maket,is_mnplt_1prc,
							                             date_init,poddon_id,warehouse_id,buyer_num,buyer_date,time_transp,cost_ecod_state_id,
							                             dscnt_disposal,disposal_on,bonus_free,sum_from_1c,address_id,agreement_new,is_logistics,
							                             ecod_confirmation,[type],delivery_type_ecod,transp_types_ecod,delivery_instruction_ecod,
							                             location_name_ecod,locat_str_number_ecod,location_city_ecod,final_buyer_ecod,order_ext,
							                             prioritet
							                             FROM nefco.dbo.zakaz_hat WHERE id = @order_id; 
							DELETE FROM nefco.dbo.loading_palletz_zakaz_hat WHERE zakaz_id = @order_id 
							UPDATE nefco.dbo.zakaz_hat_1c set deleted = 1 where id = @order_id; 
							DELETE FROM nefco.dbo.zakaz_hat WHERE id = @order_id 
							INSERT INTO nefco.dbo.order_control_logs VALUES('admin', @order_id, '', 1, GETDATE(), '"+comment+"');";
			
			return query;
		}
		
		/// <summary>
		/// Новый атрибут для материалов
		/// </summary>
		/// <param name="name">название атрибута</param>
		/// <param name="attr">номер атрибута</param>
		/// <param name="zavod">код завода 1-НК, 2- КЖК, НБП</param>
		/// <returns>Строка запроса</returns>
		public string new_material_attr(string name, int attr, int zavod)
		{
			string query=@"insert into nefco.dbo.co_material_attr_details (name, material_attr_id, factory_id) values('"+name+"',"+attr+","+zavod+");";
			return query;
		}
		
		/// <summary>
		/// Новая транспортная компания
		/// </summary>
		/// <param name="name">название транспортной</param>
		/// <param name="zavod">код завода 1-НК, 2- КЖК, НБП</param>
		/// <param name="dogovor">номер договора</param>
		/// <param name="address">юридический адрес</param>
		/// <param name="r_s">расч/счет</param>
		/// <param name="bank">наименование банка</param>
		/// <param name="k_s">кор/счет</param>
		/// <param name="bik">БИК банка</param>
		/// <param name="inn">ИНН</param>
		/// <param name="kpp">КПП</param>
		/// <param name="date_n">дата договора</param>
		/// <param name="pass">пароль</param>
		/// <param name="login">логин</param>
		/// <param name="userID">идентификатор сотрудника, который добавил ТЭК</param>
		/// <returns>Строка запроса</returns>
		public string new_TEK(string name, int zavod,string dogovor, string address, string r_s, string bank, string k_s, string bik, string inn, string kpp, DateTime date_n, string pass, string login, int userID, int clas)
		{
			string query=@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
						DECLARE @ALL_IS_OK int = 0;
						
						DECLARE @name_tek varchar(500) = '"+name+"';"+
						"DECLARE @factory_id int = "+zavod+";"+
						@"
						DECLARE @contract_number varchar(50) = '"+dogovor+"'; "+
						"DECLARE @address varchar(250) = '"+address+"'; "+
						"DECLARE @settlement_account varchar(50) = '"+r_s+"'; "+
						"DECLARE @bank_name varchar(256) = '"+bank+"';"+
						"DECLARE @loro_account varchar(50) = '"+k_s+"'; "+
						"DECLARE @bik varchar(50) = '"+bik+"'; "+
						"DECLARE @inn varchar(50) = '"+inn+"'; "+
						"DECLARE @kpp varchar(50) = '"+kpp+"'; "+
						"DECLARE @contract_date datetime = '"+date_n.ToShortDateString()+"'; "+
						@"DECLARE @contractor_id INT;
						
						DECLARE @pass varchar(50) = '"+pass+"';"+
						@"DECLARE @id INT;
						DECLARE @login varchar(50) = '"+login+"';"+
						@"
						IF NOT EXISTS(SELECT login FROM nefco.dbo.co_contractor_attr_transp WHERE login=@login)
						BEGIN
							SET @login = @login ;
						END
						ELSE
						BEGIN
							SET @login = @login + '1';
						END
						
						
						BEGIN TRANSACTION New_tek
						BEGIN TRY	
							INSERT INTO nefco.dbo.co_contractor(name,      class,  del, factory_id,  active)
														VALUES (@name_tek, " + clas + ",     0,   @factory_id, 1)"+


							@"SET @contractor_id = (SELECT IDENT_CURRENT('nefco.dbo.[co_contractor]'));
						
							INSERT INTO nefco.dbo.co_contractor_attr_transp
												(contractor_id,  contract_number,  address,  settlement_account,  bank_name,  loro_account,  bik,  inn,  kpp,  contract_date) 
										 VALUES (@contractor_id, @contract_number, @address, @settlement_account, @bank_name, @loro_account, @bik, @inn, @kpp, @contract_date)
						
							SET @id = (SELECT IDENT_CURRENT('nefco.dbo.[co_contractor_attr_transp]'));
						
							UPDATE nefco.dbo.co_contractor_attr_transp SET user_id = @id, login = @login, password = @pass WHERE id = @id
						
							
						
						  DECLARE @log_id INT;
						  DECLARE @user_id INT = " + userID+";"+
						  @"DECLARE @prgm_id INT = 6;              
						  DECLARE @tag_id INT = 120;            
						
						  EXEC nefco.dbo.co_log_insert @user_id, @prgm_id, @tag_id, @contractor_id, @log_id OUTPUT;
						  DECLARE @log_detail_id INT;
						  EXEC nefco.dbo.co_log_detail_insert @log_id, 'наименование', @name_tek, @log_detail_id OUTPUT;
						  EXEC nefco.dbo.co_log_detail_insert @log_id, 'класс', "+clas+", @log_detail_id OUTPUT;"+
						  @"EXEC nefco.dbo.co_log_detail_insert @log_id, 'завод', @factory_id, @log_detail_id OUTPUT;
						  EXEC nefco.dbo.co_log_detail_insert @log_id, 'Номер договора', @contract_number, @log_detail_id OUTPUT;
						  EXEC nefco.dbo.co_log_detail_insert @log_id, 'Адрес', @address, @log_detail_id OUTPUT; 
						  EXEC nefco.dbo.co_log_detail_insert @log_id, 'Расчетный счет', @settlement_account, @log_detail_id OUTPUT;
						  EXEC nefco.dbo.co_log_detail_insert @log_id, 'имя банка', @bank_name, @log_detail_id OUTPUT;
						  EXEC nefco.dbo.co_log_detail_insert @log_id, 'кор. счет', @loro_account, @log_detail_id OUTPUT;
						  EXEC nefco.dbo.co_log_detail_insert @log_id, 'бик', @bik, @log_detail_id OUTPUT;
						  EXEC nefco.dbo.co_log_detail_insert @log_id, 'инн', @inn, @log_detail_id OUTPUT;
						  EXEC nefco.dbo.co_log_detail_insert @log_id, 'кпп', @kpp, @log_detail_id OUTPUT;
						  EXEC nefco.dbo.co_log_detail_insert @log_id, 'дата договора', @contract_date, @log_detail_id OUTPUT;
						
						  SET @ALL_IS_OK = 1;
						END TRY	
						BEGIN CATCH
							ROLLBACK TRANSACTION New_tek
							print 'Error!' + CONVERT(VARCHAR, ERROR_NUMBER()) + ':' + ERROR_MESSAGE();
						END CATCH
						
						IF @ALL_IS_OK = 1 COMMIT TRANSACTION New_tek";
			
			return query;
		}
		
		/// <summary>
		/// Поиск транспортной
		/// </summary>
		/// <param name="name">наименование транспортной</param>
		/// <returns>Строка запроса</returns>
		public string search_TEK(string name)
		{
			string query=@"select cc.del, cc.factory_id, cc.class, cc.active, cc.name,
							ccac.contractor_id, 
							ccac.contract_number, 
							ccac.address,
							ccac.settlement_account,
							ccac.bank_name,
							ccac.loro_account,
							ccac.bik,
							ccac.inn,
							ccac.kpp,
							ccac.contract_date,
							ccac.id,
							ccac.login,
							ccac.password,
							ccac.book_time,
							ccac.limit,
							ccac.email,
							ccac.time_slot_email,
							ccac.delay,
							ccac.booking_period,
							ccac.use_common_price,
							ccac.auction_access,
							ccac.auction_user_id,
							ccac.auto_use_price_list
							from
							nefco.dbo.co_contractor cc 
							join nefco.dbo.co_contractor_attr_transp ccac on ccac.contractor_id=cc.id
							
							where cc.name like '%"+name+"%'";
			return query;
		}
		
		/// <summary>
		/// Заведение новой карточки клиента Нэфиса в базе nefco
		/// </summary>
		/// <param name="zavod">завод</param>
		/// <param name="inn">инн клиента</param>
		/// <param name="name">название клиента</param>
		/// <param name="address">адрес клиента</param>
		/// <param name="typeTT">код типа карточки клиента</param>
		/// <param name="kanal">код канала сбыта</param>
		/// <param name="sector">код сектора</param>
		/// <param name="contactor">код дистрибьютора</param>
		/// <returns>Строка запроса</returns>
		public string create_newTT(int zavod, string inn, string name, string address, int typeTT, int kanal, int sector, int contactor)
		{
			string query=@"INSERT INTO nefco.[dbo].[client_card] 
						([factory_id],[INN],[client_name],[client_adress],[urname],[uradress],[type_id],[category_id],[emp_id],[sector_id],[distr_id],[group_id],[contractor_id]) 
						VALUES
						("+zavod+",'"+inn+"','"+name+"','"+address+"','"+name+"','"+address+"',"+typeTT+","+kanal+",1317,"+sector+",(select distr_id from nefco.dbo.co_contractor_attr_customer where contractor_id="+contactor+"),(SELECT id FROM nefco.dbo.client_card_group WHERE self_type = 1 AND client_type_id = "+typeTT+" AND factory_id = "+zavod+"),"+contactor+");";
			
			return query;
		}

		/// <summary>
		/// новый класс прайс-листа
		/// </summary>
		/// <param name="name">название</param>
		/// <param name="parent">тип класса</param>
		/// <param name="zavod">завод</param>
		/// <returns>Строка запроса</returns>
		public string New_priselist_clas(string name, int parent, int zavod)
        {
			string query = @"INSERT INTO nefco.dbo.co_pricelist_class (name, parent, factory_id) 
							VALUES 
							('" + name + "', " + parent + ", " + zavod + ");";

			return query;

		}
	}
}