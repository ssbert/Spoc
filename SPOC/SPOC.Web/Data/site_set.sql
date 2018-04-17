
insert into users(id,userLoginName,userPassWord,userFullName,userMobile,userEmail,userGender,userBirthday,userIdcard,userNational,userPolitical,identity,smallAvatar,mediumAvatar,largeAvatar,about,signature,newMessageNum,newNotificationNum,approvalStatus,loginIp,loginTime,userEnbleFlag,isCompleted,sessionId,newMoocUserId) 
select '426c515b-4fe4-11e6-83a1-00ffa2a6b6c2', 'test', '6ED5833CF35286EBF8662B7B5949F0D742BBEC3F', null, '', '', null, null, null, null, null, '1', '/files/UserInfo/samilImgTotal.jpg', null, null, null, null, null, null, 'approving', '::1', now(), '0' ,1,'','00000000-0000-0000-0000-000000000000'
from dual 
where not exists (select * from users where id='426c515b-4fe4-11e6-83a1-00ffa2a6b6c2');
alter table users alter column newMoocUserId set default '00000000-0000-0000-0000-000000000000';
alter table users alter column approvalStatus set default 'approved';

UPDATE  users  SET  isCompleted=1 where isCompleted  is null;


insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'3885f32f-8e29-48a2-bf94-0782c6b7b65a', 'sys_register', 'user_register_dispaly', '学生注册', 'true', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='user_register_dispaly');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'2c3e56c0-f207-4aa0-8c84-0e501f8bf7ea', 'sys_register', 'register_userterms', '注册协议', '<p class="msonormal" style="text-align:center;">	<span style="font-family:微软雅黑;font-weight:bold;font-size:28.0000pt;">本网络平台服务使用协议</span><span style="font-family:微软雅黑;font-weight:bold;font-size:28.0000pt;"></span></p><p class="msonormal">	<span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;">1、本网站服务条款的确认和接纳</span><span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">本网站的各项网络服务的所有权、运作权归深圳市博思特教育科技有限公司。本网站提供的服务将完全按照其发布的章程、服务条款和操作规则严格执行。您必须完全同意所有服务条款并完成注册（报名）程序。&nbsp;</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal">	<span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;">2、服务简介&nbsp;</span><span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">基于本网站所提供的网络服务的重要性，学员应同意：</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">（1）提供详尽、准确的个人资料。</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">（2）不断更新注册（报名）资料,符合及时、详尽、准确的要求。中华会计网校不公开学员的姓名、地址、电子邮箱和笔名，&nbsp;除以下情况外：</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="newstyle15" style="margin-left:63.5000pt;text-indent:-21.0000pt;">	<span style="font-family:wingdings;font-size:10.5000pt;"><span>l<span>&nbsp;</span></span></span><span style="font-family:微软雅黑;font-size:10.5000pt;">学员授权本网站透露这些信息。</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="newstyle15" style="margin-left:63.5000pt;text-indent:-21.0000pt;">	<span style="font-family:wingdings;font-size:10.5000pt;"><span>l<span>&nbsp;</span></span></span><span style="font-family:微软雅黑;font-size:10.5000pt;">相应的法律及程序要求本网站提供学员的个人资料。如果学员提供的资料包含有不正确的信息，本网站保留结束学员使用网络服务资格的权利。&nbsp;</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal">	<span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;">3、服务条款的修改和服务修订&nbsp;</span><span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">本网站有权在必要时修改服务条款，本网站服务条款一旦发生变动，将会在重要页面上提示修改内容。如果学员继续享用网络服务，则视为接受服务条款的变动。&nbsp;</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal">	<span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;">4、学员账号、代码、密码及使用限制&nbsp;</span><span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">学员一旦注册（报名）成功，成为本网站的合法学员，将得到一个账号、密码和学员代码，学员将对学员代码、密码安全及学员代码的使用负全部责任；</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">同时，每个学员的学习代码仅限学员个人私自使用，学员以任何方式与任何第三方共享学习代码或公开学习课程（包括但不限于向任何第三方透露学习课程、与他人共享学习代码、将自己的学习代码提供给第三方使用、将学习课程公开播放或以任何方式供多人同时使用）都是严格禁止的；</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">上述情况一旦发生，我司将立即停止违规代码的学习权限，同时我司会进一步追究违规人员的法律责任，包含不限于追偿损失、司法追责等。</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal">	<span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;">5、对学员信息的存储和限制&nbsp;</span><span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">本网站不对学员所发布信息的删除或储存失败负责。本网站有判定学员的行为是否符合本网站服务条款的要求和精神的保留权利，如果学员违背了服务条款的规定，本网站有中断对其提供网络服务的权利。&nbsp;</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal">	<span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;">6、学员管理&nbsp;</span><span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">严禁发表、散布、传播任何反动、色情及违反国家安全、扰乱社会秩序等有害信息，学员需对自己在网上的行为承担法律责任。学员若在本网站上散布和传播反动、色情或其他违反国家法律的信息，本网站的系统记录将作为学员违反法律的证据。&nbsp;</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal">	<span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;">7、平台学费</span><span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">本平台的任何学员，统一采用在线支付方式进行缴费，一经缴费后将自动享受该课程学习权利，该学员只要有任何学习记录，学费一概不退。</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal">	<span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;">8、开始/结束服务&nbsp;</span><span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">学员付费经公司确认后，开通相应的服务权限（服务权限是指学员享受所购买服务的资格）。具体服务内容开通的时间和进度以网站的最新公告或课件更新记录为准。</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">学员或本网站可随时根据实际情况中断一项或多项网络服务。本网站不需对任何个人或第三方负责而随时中断服务。学员对后来的条款修改有异议，或对本网站的服务不满，可以行使如下权利：</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">(1)停止使用中本网站的网络服务。</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">(2)通告本网站停止对该学员的服务。</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">结束学员服务后，学员使用网络服务的权利马上中止。&nbsp;</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal">	<span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;">9、学员咨询&nbsp;</span><span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">在如下时间里您可拨打客服咨询电话：</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">免费咨询热线：4008883328　咨询服务电话：</span><span style="font-family:微软雅黑;font-size:10.5000pt;">0755-26532099</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">咨询时间：全天5&nbsp;x&nbsp;8小时服务（节假日正常休息）&nbsp;</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal">	<span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;">10、网络服务内容的所有权&nbsp;</span><span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">本网站定义的网络服务内容包括：文字、软件、声音、图片、录像、图表、邮件及广告中的全部内容，本网站拥有以上内容的完全版权，严禁任何个人或单位在未经本网站许可的情况下对这些内容进行翻版、复制、转载、篡改等一切用于商业活动的行为；本网站的学员/会员账号只为本网校的个人注册用户本人所专有，严禁一个账号多人使用，如若发现上述情况，本网站将有权停止其账号使用，并没收其非法所得，并根据情节的严重程度对其实行相应罚款或述诸法律。&nbsp;</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal">	<span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;">11、免责条款&nbsp;</span><span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">当本网站以链接形式推荐其他网站内容时，由于本站并不控制相关网站和资源，因此访问者需理解并同意，本站并不对这些网站或资源的可用性负责，且不保证从这些网站获取的任何内容、产品、服务或其他材料的真实性、合法性，对于任何因使用或信赖从此类网站或资源上获取的内容、产品、服务或其他材料而造成（或声称造成）的任何直接或间接损失，本站均不承担任何责任。</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">由于用户将个人密码告知他人或与他人共享注册帐户，由此导致的任何个人资料泄露，本网站不负任何责任。</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">任何由于黑客攻击、计算机病毒侵入或发作、因政府管制而造成的暂时性关闭等影响网络正常经营的不可抗力而造成的个人资料泄露、丢失、被盗用、被窜改或不能正常看课等，本网站均得免责。</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">本网站如因系统维护或升级而需暂停服务时，将事先公告。若因线路及非本公司控制范围外的硬件故障或其它不可抗力而导致暂停服务，于暂停服务期间造成的一切不便与损失，本网站不负任何责任。</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">本网站使用者因为违反本声明的规定而触犯中华人民共和国法律的，一切后果自己负责，本网站不承担任何责任。</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">本声明未涉及的问题参见国家有关法律法规，当本声明与国家法律法规冲突时，以国家法律法规为准。&nbsp;</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal">	<span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;">12.其他</span><span style="font-family:微软雅黑;font-weight:bold;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">我司在法律允许最大范围对本服务条款拥有最终解释权与修改权。&nbsp;</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">请您在发现任何违反本服务协议以及其他任何单项服务的服务条款之情形时，通知我们。您可以通过如下联络方式同我们联系：&nbsp;</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">深圳市博思特教育科技有限公司</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">电话：0755-26532099</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">传真：0755-26532089-832</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">地址：深圳市南山区科发路8号金融服务技术创新基地2栋9ab单元</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">邮编：518057</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p><p class="msonormal" style="text-indent:21.2000pt;">	<span style="font-family:微软雅黑;font-size:10.5000pt;">电子邮箱：sales@newv.com.cn</span><span style="font-family:微软雅黑;font-size:10.5000pt;"></span></p>', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='register_userterms');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'6ff794ae-b865-45ce-91a6-2e74c3cae50e', 'sys_register', 'register_email_activation_title', '新用户激活邮件标题', '请激活您的{{sitename}}帐号', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='register_email_activation_title');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'5bb37b91-56aa-42f0-8e1b-f26095506122', 'sys_register', 'register_email_activation_body', '新用户激活邮件内容', 'hi, {{nickname}}\r\n欢迎加入{{sitename}}!\r\n请点击下面的链接完成注册：{{verifyurl}}\r\n如果以上链接无法点击，请将上面的地址复制到你的浏览器(如ie)的地址栏中打开，该链接地址24小时内打开有效。\r\n感谢对{{sitename}}的支持！\r\n{{sitename}} {{siteurl}}\r\n(这是一封自动产生的email，请勿回复。)', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='register_email_activation_body');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'cd9b3800-4936-43ff-b155-6398db9398f2', 'sys_register', 'register_welcome_enabled', '发送欢迎信息', 'closed', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='register_welcome_enabled');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'd9424661-8131-41a4-831a-a9926b6d3344', 'sys_register', 'register_welcome_sender', '欢迎信息发送方', '欢迎加入{{sitename}}', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='register_welcome_sender');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'7e92d60a-35f7-4009-81bb-a81e6f96ed3a', 'sys_register', 'register_welcome_title', '欢迎信息标题', '欢迎加入{{sitename}}', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='register_welcome_title');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'ce1d6362-2c95-479a-8978-17b7a10f7f8b', 'sys_mailer', 'is_send_email', '邮件发送', 'closed', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='is_send_email');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'371e0af5-e1d7-469e-acc5-b3c52984a8f9', 'sys_mailer', 'mailer_host', 'smtp服务器地址', 'mail.newv.com.cn', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='mailer_host');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'5db41f65-836e-4202-8b47-1a12e7759d1a', 'sys_mailer', 'mailer_username', 'smtp用户名', '', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='mailer_username');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'31192942-8be9-4a24-8327-eca97c1f2e88', 'sys_mailer', 'mailer_password', 'smtp密码', '', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='mailer_password');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'accc13fc-4074-4b5a-aded-d4a333311df6', 'sys_mailer', 'mailer_from', '发信人地址', '', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='mailer_from');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'9c1bbc40-3272-4405-8253-c6c0777dbb60', 'sys_mailer', 'mailer_name', '发信人名称', '深圳市博思特教育科技有限公司', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='mailer_name');


insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'6c67983b-a024-4bdd-887d-d4bc37825ba1', 'sys_mailer', 'mailer_hostPort', 'smtp端口号', '25', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='mailer_hostport');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'df85f85e-4400-4217-be5a-aaa542fa14c2', 'sys_login', 'first_login_perfect_userdata', '首次登陆完善个人资料', 'true', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='first_login_perfect_userdata');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'53cbcf79-f04c-4267-be3e-ea4d59ed5efe', 'sys_login', 'allow_nologin_access', '允许未登录用户访问', 'true', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='allow_nologin_access');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'c28a7b2b-4126-4bb5-9d56-843c54a3becf', 'sys_login', 'login_limit', '用户登录限制', 'closed','','2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='login_limit');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'217502b6-2c06-48cf-80a3-bfe14919eb30', 'sys_login', 'login_enabled', '第三方登录', 'closed', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='login_enabled');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'7ac8fcc6-1de8-46ef-b181-159a21ec514c', 'sys_login', 'login_weibo_enabled', '微博登录', 'closed', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='login_weibo_enabled');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'35f8b31a-2877-41d9-b16b-255ce95df9df', 'sys_login', 'login_weibo_key', 'app key',NULL, '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='login_weibo_key');


insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'ff6a9c62-0820-4a64-b072-871114af2261', 'sys_login', 'login_weibo_secret', 'app secret', NULL,'', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='login_weibo_secret');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'd3044f62-4e8b-4c6b-9f82-268f8fbe8695', 'sys_login', 'login_qq_enabled', 'qq登录', 'closed', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='login_qq_enabled');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'dab81027-8d2a-4b7e-9d4b-c0da91045c87', 'sys_login', 'login_qq_key', 'app id',NULL,  '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='login_qq_key');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'e2bd1756-6265-47ea-875b-ccf255d2a2e1', 'sys_login', 'login_qq_secret', 'app secret',NULL,  '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='login_qq_secret');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'5a180b38-c490-4fb4-b325-fb8c4da815a0', 'sys_login', 'login_renren_enabled', '人人连接', 'closed', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='login_renren_enabled');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'0c79d4e5-175b-4b5a-a6c5-9879ac67d8e7', 'sys_login', 'login_renren_key', 'app key', NULL,'', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='login_renren_key');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'b10ce7a6-8595-4f72-8ed0-494e9f06f1f5', 'sys_login', 'login_renren_secret', 'app secret',NULL, '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='login_renren_secret');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select  
'623da4a1-dbc2-498c-bdf5-480f3744c0c0', 'sys_login', 'login_verify_code', '验证代码',NULL, '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='login_verify_code');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'426b9a8c-ef07-4d37-9874-3798765f1cd9', 'sys_usercenter', 'username_contain_EN', '用户名一定要包含英文+数字', 'closed', '', '2015-03-27 14:06:58', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='username_contain_en');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'4494f1a9-8766-4519-89c3-b8ed32a21fa7', 'sys_mailer', 'mailer_enabled', '邮件发送', 'closed', '', '2015-03-27 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='mailer_enabled');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'd85f20d7-f830-462b-b235-eca95980533f', 'sys_usercenter', 'user_for_register_dispaly', '注册报名', 'false', '', '2015-03-27 14:06:58', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='user_for_register_dispaly');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'ab7ff2af-9a27-4015-80e9-bf069e803119', 'sys_usercenter', 'user_register_isapprove', '注册是否需要审批', 'false', '', '2015-03-27 14:06:58', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='user_register_isapprove');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'174b6d61-0d5d-4bad-aa6d-835bf0345d5a', 'sys_usercenter', 'allow_edit_avatar_picture', '允许用户修改头像', 'true', '', '2015-03-27 14:06:58', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='allow_edit_avatar_picture');


insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'487caad3-16df-4111-a551-8828806f282f', 'sys_usercenter', 'invitation_code_support', '支持邀请码', 'false', '', '2015-03-27 14:06:58', 'y'
 from dual
where not exists (select * from site_set
where settingkey='invitation_code_support');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'174b6d61-0d5d-4bad-aa6d-835bf0345d5b', 'sys_usercenter', 'invitation_code_needed', '邀请码是否必填', 'false', '', '2015-03-27 14:06:58', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='invitation_code_needed');


insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'487caad3-16df-4111-a551-8828806f282c', 'sys_usercenter', 'invitation_code_allowed_show', '允许学生显示邀请码', 'false', '', '2015-03-27 14:06:58', 'y'
 from dual
where not exists (select * from site_set
where settingkey='invitation_code_allowed_show');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'15aac90f-bd1a-11e6-b23a-d4ae528cba72', 'sys_login', 'site_Login', '本站登录限制', 'false', '', '2015-03-27 14:06:58', 'y'
 from dual
where not exists (select * from site_set
where settingkey='site_Login');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'8608736d-3363-4b90-a557-8d6f029b924d', 'course_type', 'credential', '认证课程', 'credential', '', '2017-07-31 14:24:00', 'y'
 from dual
where not exists (select * from site_set
where settingkey='credential');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'426bfe7f-332a-4803-b5d5-82147e1be7fc', 'course_type', 'credit', '学分课程', 'credit', '', '2017-07-31 14:24:00', 'y'
 from dual
where not exists (select * from site_set
where settingkey='credit');

insert into site_set (id, settinggroup, settingkey, settingname, settingvalue, settingremark, modifytime, isvisible) select 
'a034af88-de4b-11e7-883f-7824af8c98ea', 'sys_usercenter', 'allow_paste_code', '允许代码编辑器粘贴', 'false', '', '2017-12-01 14:06:57', 'y'
 from dual
where not exists (select * from site_set
where site_set.settingkey='allow_paste_code');

insert into site_set select 'ecef0774-c1c6-4d25-a234-efd35313ead0','sys_site','max_point_rate','最大得分比率','50','得分点最大得分比率',SYSDATE(),'y' from dual where not exists 
(select * from site_set where id='ecef0774-c1c6-4d25-a234-efd35313ead0');

