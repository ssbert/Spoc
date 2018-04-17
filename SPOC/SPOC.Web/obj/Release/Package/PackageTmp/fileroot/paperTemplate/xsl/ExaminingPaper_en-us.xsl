<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
 xmlns:user="http://www.wrox.com.uuid">
  <xsl:output encoding="UTF-8" method="html"/>
  <xsl:param name="exam_do_mode_code"></xsl:param>
  <xsl:param name="is_need_limited_time"></xsl:param>
  <xsl:param name="exam_grade_uid"></xsl:param>
  <xsl:param name="exam_uid"></xsl:param>
  <xsl:param name="exam_is_only_upload_file"></xsl:param>
  <xsl:param name="current_paper_node_uid"></xsl:param>
  <xsl:param name="current_question_uid"></xsl:param>
  <xsl:variable name="is_show_score" select="//exam_paper_object/exam_paper/is_show_score"></xsl:variable>
  <xsl:variable name="is_single_as_multi" select="//exam_paper_object/exam_paper/is_single_as_multi"></xsl:variable>
  <msxsl:script language="javascript" implements-prefix="user">
    <![CDATA[
    
    //填空题，把下划线替换成文本框（下划线个数为3个以上）
    function ReplaceFillInContent(Content,questionUid)
    {
      var lowLinePos = Content.indexOf("_");
      var fillInBoxCount = 0;
      var preLowLinePos = -1;		//上一个下划线位置
      var lowLineCount = 0;
      var oneAnswer="";
      var oneInputBoxString = "";
      while (lowLinePos > -1)
      {
          //如果前一个下划线不是前一个字符
          if (lowLinePos != preLowLinePos + 1)
          {
              //如果下划线个数大于3
              if (lowLineCount >= 3)
              {
                  oneInputBoxString = "<input type='text' class='QuestionInputText' id='Answer_" + questionUid + "' name='Answer_" + questionUid + "'  maxlength='1950' class='LineText' style='overflow:visible;BACKGROUND-COLOR: #ffff66;width:" + (lowLineCount * 30) + "' value='" + oneAnswer + "' onkeyup=\"CheckFillQuestionAnswerLength(this)\" onchange=\"SetQuestionAnswerStatus('"+questionUid+"',true)\" />";
                  Content = Content.substring(0, preLowLinePos - lowLineCount + 1) + oneInputBoxString + Content.substring(preLowLinePos + 1);
                  //重设原来那个下划线的位置
                  lowLinePos = lowLinePos + oneInputBoxString.Length - lowLineCount;
                  //fillInBoxCount = fillInBoxCount + 1;
              }
              //重新开始
              lowLineCount = 1;
          }
          else
          {
              lowLineCount = lowLineCount + 1;
          }
          preLowLinePos = lowLinePos;
          lowLinePos = Content.indexOf("_", lowLinePos + 1);
      }
      if (lowLineCount >= 3)
      {
          //找到一个填空题
          oneInputBoxString = "<input type='text' class='QuestionInputText' id='Answer_" + questionUid + "' name='Answer_" + questionUid + "'  maxlength='1950' class='LineText' style='overflow:visible;BACKGROUND-COLOR: #ffff66;width:" + (lowLineCount * 30) + "' value='" + oneAnswer + "' onkeyup=\"CheckFillQuestionAnswerLength(this);\" onchange=\"SetQuestionAnswerStatus('"+questionUid+"',true)\" />";
          Content = Content.substring(0, preLowLinePos - lowLineCount + 1) + oneInputBoxString + Content.substring(preLowLinePos + 1);
          //重设原来那个下划线的位置
          lowLinePos = lowLinePos + oneInputBoxString.Length - lowLineCount;
          fillInBoxCount = fillInBoxCount + 1;
      }
      //下面一定要带上+"",原因不知

      return Content+"";
    }
    
    //大题序号，把小写阿拉伯数字转为中文汉字
    function Arabia_to_Chinese(num)
    {   
        if(isNaN(num)) return num;   
           
        var a=[["","I","II","III","IV","V","VI","VII","VIII","IX"],   
                      ["","X","XX","XXX","XL","L","LX","LXX","LXXX","XCC"],   
                      ["","C","CC","CCC","CD","D","DC","DCC","DCCC","CM"]];   
        var roman = "";   
        var t=0;   
        for(var m=0, i=1000; m<3; m++,i/=10){   
            t=Math.floor((num%i)/(i/10));   
            roman += a[2-m][t];   
        }   
        return roman; 
    }     
    
    //把试题选项序号转换成A,B,C,D...
    function Arabia_to_English(Num)
    {
        Num = Num + 64;
        return String.fromCharCode(Num);
    }
    
    var _gloableQuestionIndex=0;
    function AddCurrentGloableQuestionIndex()
    {
      _gloableQuestionIndex=_gloableQuestionIndex+1;
      return "";
    }
    
    function GetCurrentGloableQuestionIndex()
    {
        return _gloableQuestionIndex+"";
    }
    
    var _navigatorHtml="";
    var _navigatorIndex=1;
    function AddPaperNodeToNavigator(paperNodeIndex)
    {
      _navigatorIndex=1;
      //if(paperNodeIndex!="1")
      _navigatorHtml=_navigatorHtml+"<br />";
      _navigatorHtml=_navigatorHtml+"<span style='font-size:12px;'><b>Part "+Arabia_to_Chinese(paperNodeIndex)+"</b></span>";
      //生成表格头
      _navigatorHtml=_navigatorHtml+"<table width='180' border='0' align='center' cellpadding='0' cellspacing='1' bgcolor='#E1F4FF'>";
      _navigatorHtml=_navigatorHtml+"<tr>";
      return "";
    }
    
    function EndAddPaperNodeToNavigator()
    {
      //补全Td
      for(;_navigatorIndex<=6;_navigatorIndex++)
      {
        _navigatorHtml=_navigatorHtml+"<td width='30' height='30' class='TdNoQuestionBookmarkText_No_Answer' align='center'> </td>";
      }
      _navigatorHtml=_navigatorHtml+"</tr></table>";
      return "";
    }
    
    function AddQuestionToNavigator(questionUid,paperNodeIndex,parenntQuestionIndex,questionIndex,questionIndexView)
    {
	    if(questionIndexView!="")
      {
        if(_navigatorIndex!=1 && _navigatorIndex%6==1)
        {
          _navigatorHtml=_navigatorHtml+"</tr><tr>";
          _navigatorIndex=1;
        }
	      _navigatorHtml=_navigatorHtml+"<td onclick=\"javascript:iframePaper.GoToQuestion('"+paperNodeIndex+"','"+parenntQuestionIndex+"','"+questionIndex+"')\" width='30' height='30' id='tdQuestionVavigator_"+questionUid+"' class='TdNoQuestionBookmarkText_No_Answer' align='center'><a id='lnkVavigator_"+questionUid+"' href=\"javascript:iframePaper.GoToQuestion('"+paperNodeIndex+"','"+parenntQuestionIndex+"','"+questionIndex+"')\" class='QuestionNavigatorText_No_Answer' title='"+paperNodeIndex+"_"+parenntQuestionIndex+"_"+questionIndex+"'>"+questionIndexView+"</a></td>";
        _navigatorIndex=_navigatorIndex+1;
      }
      return "";
    }
    
    function GetNavigatorHtml()
    {
      return _navigatorHtml;
    }
    
    function AddString(str1,str2)
    {
      return str1+str2;
    }
    
    function TrimQuoteForTypingQuestion(question_text)
    {
      return question_text.replace(/\\"/g,"");    //将"号替换成为空
    }
	
	function GetJudgeOptionText(answer)
    {
		if(answer=="Y"){
	return "Accurate";
		}
		if(answer=="N"){
	return "Error";
		}
	return "";
    }
    ]]>
  </msxsl:script>

  <xsl:template match = "/">

    <xsl:apply-templates select="exam_paper_object"/>

  </xsl:template>

  <xsl:template match = "exam_paper_object">
    <TABLE BORDER="0" WIDTH="98%" align="center"  cellpadding="0" cellspacing="0">
      <TR>
        <TD align="center" COLSPAN="2" class="PaperTitleText">
          <xsl:element name="input">
            <!-- 保存PaperUid到hidPaper中 -->
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidPaperUid</xsl:attribute>
            <xsl:attribute name="name">hidPaperUid</xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="exam_paper/paper_uid"/></xsl:attribute>
          </xsl:element>

          <xsl:element name="input">
            <!-- 保存each_option_score -->
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidEachOptionScore</xsl:attribute>
            <xsl:attribute name="name">hidEachOptionScore</xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="exam_paper/each_option_score"/></xsl:attribute>
          </xsl:element>

          <xsl:element name="input">
            <!-- 保存is_single_as_multi -->
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidIsSingleAsMulti</xsl:attribute>
            <xsl:attribute name="name">hidIsSingleAsMulti</xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="exam_paper/is_single_as_multi"/></xsl:attribute>
          </xsl:element>
          
          <!-- 考试名称 -->
          <!--font size="5">
            <xsl:value-of select="exam_paper/paper_name"/>
          </font-->
          <!-- //考试名称 -->
        </TD>
      </TR>
      <!--
      <TR>
        <TD align="left" valign="bottom" wrap="false" COLSPAN="2">
          总共<xsl:value-of select="exam_paper/question_num"></xsl:value-of>题　
          共<xsl:value-of select="exam_paper/total_score"></xsl:value-of>分
        </TD>
      </TR>
      <TR height="1px" bgcolor="#3a86c4">
        <TD></TD>
        <TD></TD>
      </TR>
      
      <TR height="10px">
        <TD>
          <TD></TD>
        </TD>
      </TR>
-->
      <xsl:for-each select="exam_paper_nodes/exam_paper_node">
        <xsl:call-template name="exam_paper_node">
        </xsl:call-template>
        <xsl:value-of select="user:EndAddPaperNodeToNavigator()"/>
      </xsl:for-each>

      <xsl:choose>
        <xsl:when test="$exam_do_mode_code='question'">
          <TR>
            <TD colspan="2" valign="top">
              <TABLE BORDER="0" cellpadding="0" cellspacing="0">
					<Tr>
						<td>
							<input type="button" class="Button" name="btnPreQuestion" id="btnPreQuestion"  onclick="GoToPreQuestion()"  value="previous"/></td>
						<td>
							&#160;&#160;&#160;&#160;</td>
						<td>
							<input type="button" class="Button" name="btnNextQuestion" id="btnNextQuestion" onclick="GoToNextQuestion()"  value="next"/></td>
					</Tr>
              	</TABLE>
            </TD>
          </TR>
        </xsl:when>
      </xsl:choose>
      <TR height="10px">
        <TD>
          <TD></TD>
        </TD>
      </TR>
      <TR>
        <TD align="center" COLSPAN="2" class="PaperNodeBackColor" id="tdNavigatorHidden" style="display:none">
          <xsl:value-of select="user:GetNavigatorHtml()" disable-output-escaping="yes"/>
        </TD>
      </TR>
    </TABLE>
  </xsl:template>

  <xsl:template name="exam_paper_node">
    <xsl:variable name="paper_node_index" select="string(position())"/>
    <xsl:element name="TR">
      <xsl:attribute name="id">trPaperNode_<xsl:value-of select="$paper_node_index"/></xsl:attribute>
      <xsl:attribute name="style">
        <xsl:choose>
          <xsl:when test="$exam_do_mode_code='paper'">display:block</xsl:when>
          <xsl:otherwise>display:none</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <xsl:attribute name="bgcolor">#3a86c4</xsl:attribute>
      <xsl:attribute name="onclick">jscomFlexObject(document.all('tblPaperNode_<xsl:value-of select="$paper_node_index"/>'))</xsl:attribute>
      <xsl:attribute name="height">30px</xsl:attribute>
      <TD width="10PX" class="PaperNodeBackColor"></TD>
      <TD width="99%" class="PaperNodeBackColor">
        <font color="white">
          <xsl:value-of select="user:Arabia_to_Chinese(string(position()))"/>、<xsl:value-of select="paper_node_name"/>
			（Question Totals:<xsl:value-of select="question_num"></xsl:value-of>
		  <xsl:if test="$is_show_score='Y'">
			  <xsl:if test="question_score>0">
				  ，Question Score:<xsl:value-of select="question_score"></xsl:value-of>
			  </xsl:if>
			  ，Total score:<xsl:value-of select="total_score"></xsl:value-of>
		  </xsl:if>
		  ）
          <xsl:if test="paper_node_desc!=''">
            <br /><xsl:text disable-output-escaping="yes">&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;</xsl:text><xsl:value-of select="paper_node_desc" disable-output-escaping="yes"></xsl:value-of>
          </xsl:if>
          <!-- 添加大题导航信息 -->
          <xsl:value-of select="user:AddPaperNodeToNavigator(string(position()))"/>
        </font>
      </TD>
    </xsl:element>
    <TR>
      <TD colspan="2">
        <!-- 试题 -->
        <xsl:element name="Table">
          <xsl:attribute name="id">tblPaperNode_<xsl:value-of select="$paper_node_index"/></xsl:attribute>
          <xsl:attribute name="cellpadding">0</xsl:attribute>
          <xsl:attribute name="cellspacing">0</xsl:attribute>
          <xsl:attribute name="border">0</xsl:attribute>
          <xsl:attribute name="width">100%</xsl:attribute>
          <!--xsl:attribute name="height">100%</xsl:attribute-->
          <xsl:for-each select="exam_paper_node_questions/exam_paper_node_question">
            <xsl:variable name="parent_question_index" select="string(position())"/>
            <xsl:element name="tr">
              <xsl:attribute name="id">trQuestionElement_<xsl:value-of select="$paper_node_index"/>_<xsl:value-of select="$parent_question_index"/></xsl:attribute>
			  <xsl:attribute name="style">
				<xsl:choose>
				  <xsl:when test="$exam_do_mode_code='paper'">display:block</xsl:when>
				  <xsl:otherwise>display:none</xsl:otherwise>
				</xsl:choose>
			  </xsl:attribute>
              <td align="left">
                <xsl:element name="table">
                  <xsl:attribute name="id">tblQuestion_<xsl:value-of select="$paper_node_index"/>_<xsl:value-of select="$parent_question_index"/></xsl:attribute>
                  <xsl:attribute name="width">100%</xsl:attribute>
                  <xsl:attribute name="style">
                    <xsl:choose>
                      <xsl:when test="$exam_do_mode_code='paper' or parent_question_uid!=''">display:block</xsl:when>
                      <xsl:otherwise>display:none</xsl:otherwise>
                    </xsl:choose>
                  </xsl:attribute>

                  <xsl:choose>
                    <!-- 组合题 -->
                    <xsl:when test="question_base_type_code='compose'">
                      <xsl:call-template name="exam_paper_node_question">
                        <xsl:with-param name="paper_node_index" select="$paper_node_index"></xsl:with-param>
                        <xsl:with-param name="parent_question_index" select="$parent_question_index"></xsl:with-param>
                        <xsl:with-param name="question_index" select="'0'"></xsl:with-param>
                        <xsl:with-param name="question_index_view" select="string(position())"></xsl:with-param>	<!--试题按每大题编号时用-->
                        <!-- <xsl:with-param name="question_index_view" select="'*'"></xsl:with-param> -->  <!--试题顺序号整卷编号时用-->
                      </xsl:call-template>
                      <!-- 添加试题导航信息 -->
                      <xsl:value-of select="user:AddQuestionToNavigator(string(question_uid),$paper_node_index,string(position()),'0',string(position()))"/>	<!--试题按每大题编号时用-->
                      <!--xsl:value-of select="user:AddQuestionToNavigator(string(question_uid),$paper_node_index,$parent_question_index,'0','')"/> --> <!--试题顺序号整卷编号时用-->
                      <xsl:for-each select="sub_exam_paper_node_questions/exam_paper_node_question">
                        <TR>
                          <td align="left">
                            <xsl:element name="table">
                              <xsl:attribute name="id">tblQuestion_<xsl:value-of select="question_uid"/></xsl:attribute>
                              <xsl:attribute name="width">100%</xsl:attribute>
                              <xsl:value-of select="user:AddCurrentGloableQuestionIndex()"/><!--试题计算加1-->
                              <xsl:call-template name="exam_paper_node_question">
                                <xsl:with-param name="paper_node_index" select="$paper_node_index"></xsl:with-param>
                                <xsl:with-param name="parent_question_index" select="$parent_question_index"></xsl:with-param>
                                <xsl:with-param name="question_index" select="string(position())"></xsl:with-param>
                                 <xsl:with-param name="question_index_view" select="user:AddString(string(position()),')')"></xsl:with-param> 	<!--试题按每大题编号时用-->
                                <!--<xsl:with-param name="question_index_view" select="user:GetCurrentGloableQuestionIndex()"></xsl:with-param>--> <!--试题顺序号整卷编号时用-->
                              </xsl:call-template>
                              <!-- 添加试题导航信息 -->
                              <xsl:value-of select="user:AddQuestionToNavigator(string(question_uid),$paper_node_index,$parent_question_index,string(position()),user:AddString(string(position()),')'))"/>	<!--试题按每大题编号时用-->
                              <!-- <xsl:value-of select="user:AddQuestionToNavigator(string(question_uid),$paper_node_index,$parent_question_index,string(position()),string(user:GetCurrentGloableQuestionIndex()))"/> --> <!--试题顺序号整卷编号时用-->
                            </xsl:element>
                          </td>
                        </TR>
                      </xsl:for-each>
                    </xsl:when>
                    <!-- 非组合题 -->
                    <xsl:otherwise>
                      <xsl:value-of select="user:AddCurrentGloableQuestionIndex()"/><!--试题计算加1-->
                      <xsl:call-template name="exam_paper_node_question">
                        <xsl:with-param name="paper_node_index" select="$paper_node_index"></xsl:with-param>
                        <xsl:with-param name="parent_question_index" select="$parent_question_index"></xsl:with-param>
                        <xsl:with-param name="question_index" select="string(position())"></xsl:with-param>
                         <xsl:with-param name="question_index_view" select="string(position())"></xsl:with-param> 	<!--试题按每大题编号时用-->
                        <!--<xsl:with-param name="question_index_view" select="user:GetCurrentGloableQuestionIndex()"></xsl:with-param>--> <!--试题顺序号整卷编号时用-->
                      </xsl:call-template>
                      <!-- 添加试题导航信息 -->
                      <xsl:value-of select="user:AddQuestionToNavigator(string(question_uid),$paper_node_index,$parent_question_index,string(position()),string(position()))"/>	<!--试题按每大题编号时用-->
                      <!-- <xsl:value-of select="user:AddQuestionToNavigator(string(question_uid),$paper_node_index,$parent_question_index,string(position()),string(user:GetCurrentGloableQuestionIndex()))"/> --> <!--试题顺序号整卷编号时用-->
                    </xsl:otherwise>
                  </xsl:choose>
                  <!-- //组合题 -->
                </xsl:element>
              </td>
            </xsl:element>
          </xsl:for-each>
        </xsl:element>
        <!-- //试题 -->
      </TD>
    </TR>
  </xsl:template>
  
  <xsl:template name="exam_paper_node_question">
      <xsl:param   name="paper_node_index"/>
      <xsl:param   name="parent_question_index"/>
      <xsl:param   name="question_index"/>
      <xsl:param   name="question_index_view"/>
      <TR bgcolor="#CCCCCC" height="30px">
        <td align="left" width="100%" class="PaperQuestionTextBackColor">
          <!-- QuestionUid -->
          <xsl:element name="input">
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidQuestionUid</xsl:attribute>
            <xsl:attribute name="name">hidQuestionUid</xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="question_uid"/></xsl:attribute>
          </xsl:element>
          <!-- //QuestionUid -->

          <!-- question_base_type_code -->
          <xsl:element name="input">
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidQuestionBaseTypeCode_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="name">hidQuestionBaseTypeCode_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="question_base_type_code"/></xsl:attribute>
          </xsl:element>
          <!-- //question_base_type_code -->
		  
		  <!-- question_type_uid -->
          <xsl:element name="input">
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidQustionTypeUid_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="name">hidQustionTypeUid_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="question_type_uid"/></xsl:attribute>
          </xsl:element>
          <!-- //question_type_uid -->

          <!-- standard_answer -->
          <xsl:element name="input">
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidStandardAnswer_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="name">hidStandardAnswer_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="standard_answer"/></xsl:attribute>
          </xsl:element>
          <!-- //standard_answer -->

          <!-- paper_question_score -->
          <xsl:element name="input">
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidPaperQuestionScore_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="name">hidPaperQuestionScore_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="paper_question_score"/></xsl:attribute>
          </xsl:element>
          <!-- //paper_question_score -->

          <!-- select_answer_score -->
          <xsl:element name="input">
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidSelectAnswerScore_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="name">hidSelectAnswerScore_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="select_answer_score"/></xsl:attribute>
          </xsl:element>
          <!-- //select_answer_score -->
          
          <!-- is_answer_by_html -->
          <xsl:element name="input">
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidPaperQuestionExamTime_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="name">hidPaperQuestionExamTime_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="paper_question_exam_time"/></xsl:attribute>
          </xsl:element>
          <!-- //is_answer_by_html -->
          
          <!-- is_answer_by_html -->
          <xsl:element name="input">
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidIsAnswerByHtml_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="name">hidIsAnswerByHtml_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="is_answer_by_html"/></xsl:attribute>
          </xsl:element>
          <!-- //is_answer_by_html -->
          
          <!-- operate_type_code -->
          <xsl:element name="input">
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidOperateTypeCode_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="name">hidOperateTypeCode_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="operate_type_code"/></xsl:attribute>
          </xsl:element>
          <!-- //operate_type_code -->

          <!-- 用户该题答题时间 -->
          <xsl:element name="input">
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="id">hidUserAnswerTime_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="name">hidUserAnswerTime_<xsl:value-of select="question_uid"/></xsl:attribute>
            <xsl:attribute name="value">0</xsl:attribute>
          </xsl:element>
          <!-- //用户该题答题时间 -->
          
          <!-- QuestionText -->
		  <xsl:element name="a">
		  	<xsl:attribute name="href">javascript:SetQuestionBookmark(<xsl:value-of select="$paper_node_index"/>,<xsl:value-of select="$parent_question_index"/>,<xsl:value-of select="$question_index"/>)</xsl:attribute>
			<xsl:attribute name="title">Mark /Clear</xsl:attribute>
			<xsl:text disable-output-escaping="yes">&amp;nbsp;&amp;nbsp;</xsl:text>
			<xsl:element name="span">
				<xsl:attribute name="id">spanQuestionMark_<xsl:value-of select="question_uid"/></xsl:attribute>
				<xsl:attribute name="class">QuestionNoBookmarkText</xsl:attribute><xsl:text disable-output-escaping="yes">&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;</xsl:text>
				<xsl:attribute name="onclick">javascript:SetQuestionBookmark(<xsl:value-of select="$paper_node_index"/>,<xsl:value-of select="$parent_question_index"/>,<xsl:value-of select="$question_index"/>)</xsl:attribute>
			</xsl:element>
		  </xsl:element>
		  
		  <xsl:element name="input">
			<xsl:attribute name="id">hidIsRead_<xsl:value-of select="question_uid"/></xsl:attribute>
			<xsl:attribute name="name">hidIsRead_<xsl:value-of select="question_uid"/></xsl:attribute>
			<xsl:attribute name="type">hidden</xsl:attribute>
			<xsl:attribute name="value"><xsl:value-of select="is_read"/></xsl:attribute>
		  </xsl:element>
          <xsl:element name="input">
            <xsl:attribute name="id">hidQuestion_<xsl:value-of select="$paper_node_index"/>_<xsl:value-of select="$parent_question_index"/>_<xsl:value-of select="$question_index"/></xsl:attribute>
            <xsl:attribute name="name">hidQuestion_<xsl:value-of select="$paper_node_index"/>_<xsl:value-of select="$parent_question_index"/>_<xsl:value-of select="$question_index"/></xsl:attribute>
            <xsl:attribute name="type">hidden</xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="string(question_uid)"/></xsl:attribute>
          </xsl:element>
          <xsl:element name="a">
            <xsl:attribute name="id">lnkQuestion_<xsl:value-of select="$paper_node_index"/>_<xsl:value-of select="$parent_question_index"/>_<xsl:value-of select="$question_index"/></xsl:attribute>
            <xsl:attribute name="name">lnkQuestion_<xsl:value-of select="$paper_node_index"/>_<xsl:value-of select="$parent_question_index"/>_<xsl:value-of select="$question_index"/></xsl:attribute>
            <xsl:attribute name="class">QuestionNavigatorText_No_Answer</xsl:attribute>
            <xsl:attribute name="href">javascript:SetQuestionBookmark(<xsl:value-of select="$paper_node_index"/>,<xsl:value-of select="$parent_question_index"/>,<xsl:value-of select="$question_index"/>)</xsl:attribute>
            <xsl:value-of select="$question_index_view"/>
          </xsl:element>．
          <xsl:choose>
            <xsl:when test="question_base_type_code='fill'">
              <xsl:value-of select="user:ReplaceFillInContent(string(question_text),string(question_uid))" disable-output-escaping="yes"/>
            </xsl:when>
            <xsl:when test="question_base_type_code='typing'">
              <!-- 打字题不要出内容 -->
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="question_text" disable-output-escaping="yes"/>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:choose>
            <xsl:when test="paper_question_exam_time!='0'">
			  <xsl:if test="$exam_do_mode_code='question' and $is_need_limited_time='Y'">
				  <font color="red">
					  (Time  Limit<xsl:value-of select="paper_question_exam_time"/>)
				  </font>
			  </xsl:if>
            </xsl:when>
          </xsl:choose>
          <xsl:if test="$is_show_score='Y'">
            （<xsl:value-of select="paper_question_score"></xsl:value-of> 分）
          </xsl:if>
          <!-- //QuestionText -->
        </td>
      </TR>
      <TR>
        <TD class="PaperQuestionOptionBackColor">
          <!-- 题型处理 -->
          <xsl:choose>
            <xsl:when test="question_base_type_code='single' or question_base_type_code='eva_single'">
              <!-- 单选题 -->
              <xsl:for-each select="select_answers/select_answer">
                <xsl:element name="input">
                  <xsl:attribute name="id">Answer_<xsl:value-of select="../../question_uid"/></xsl:attribute>
                  <xsl:attribute name="name">Answer_<xsl:value-of select="../../question_uid"/></xsl:attribute>
                  <xsl:choose>
                    <xsl:when test="$is_single_as_multi='Y'">
                      <xsl:attribute name="type">checkbox</xsl:attribute>
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:attribute name="type">radio</xsl:attribute>
                    </xsl:otherwise>
                  </xsl:choose>
                  <xsl:attribute name="onclick">SetQuestionAnswerStatus("<xsl:value-of select="../../question_uid"/>",true)</xsl:attribute>
                  <xsl:attribute name="value"><xsl:value-of select="select_answer_value"/></xsl:attribute>
                  <xsl:value-of select="user:Arabia_to_English(position())"></xsl:value-of>．
                  <xsl:element name="span">
                    <xsl:attribute name="id">spanSelectAnswer_<xsl:value-of select="../../question_uid"/></xsl:attribute>
                    <xsl:attribute name="onclick">jscomCheckedQuestionAnswer('Answer_<xsl:value-of select="../../question_uid"/>',<xsl:value-of select="position()-1" />);SetQuestionAnswerStatus("<xsl:value-of select="../../question_uid"/>",true)</xsl:attribute>
                    <xsl:value-of select="select_answer_text"  disable-output-escaping="yes"/>
                  </xsl:element>
                </xsl:element>
                <br></br>
              </xsl:for-each>
            </xsl:when>

            <xsl:when test="question_base_type_code='multi' or question_base_type_code='eva_multi'">
              <!-- 多选题 -->
              <xsl:for-each select="select_answers/select_answer">
                <xsl:element name="input">
                  <xsl:attribute name="id">Answer_<xsl:value-of select="../../question_uid"/></xsl:attribute>
                  <xsl:attribute name="type">checkbox</xsl:attribute>
                  <xsl:attribute name="onclick">SetQuestionAnswerStatus("<xsl:value-of select="../../question_uid"/>",true)</xsl:attribute>
                  <xsl:attribute name="value"><xsl:value-of select="select_answer_value"/></xsl:attribute>
                  <xsl:value-of select="user:Arabia_to_English(position())"></xsl:value-of>．
                  <xsl:element name="span">
                    <xsl:attribute name="id">spanSelectAnswer_<xsl:value-of select="../../question_uid"/></xsl:attribute>
                    <xsl:attribute name="onclick">jscomCheckedQuestionAnswer('Answer_<xsl:value-of select="../../question_uid"/>',<xsl:value-of select="position()-1" />);SetQuestionAnswerStatus("<xsl:value-of select="../../question_uid"/>",true)</xsl:attribute>
                    <xsl:value-of select="select_answer_text"  disable-output-escaping="yes"/>
                  </xsl:element>
                </xsl:element>
                <br></br>
              </xsl:for-each>
            </xsl:when>

            <xsl:when test="question_base_type_code='judge'">
              <!-- 判断题 -->
              <xsl:for-each select="select_answers/select_answer">
                <xsl:element name="input">
                  <xsl:attribute name="id">Answer_<xsl:value-of select="../../question_uid"/></xsl:attribute>
                  <xsl:attribute name="name">Answer_<xsl:value-of select="../../question_uid"/></xsl:attribute>
                  <xsl:attribute name="type">radio</xsl:attribute>
                  <xsl:attribute name="onclick">SetQuestionAnswerStatus("<xsl:value-of select="../../question_uid"/>",true)</xsl:attribute>
                  <xsl:attribute name="value"><xsl:value-of select="select_answer_value"/></xsl:attribute>
                  <xsl:element name="span">
                    <xsl:attribute name="id">spanSelectAnswer_<xsl:value-of select="../../question_uid"/></xsl:attribute>
                    <xsl:attribute name="onclick">jscomCheckedQuestionAnswer('Answer_<xsl:value-of select="../../question_uid"/>',<xsl:value-of select="position()-1" />);SetQuestionAnswerStatus("<xsl:value-of select="../../question_uid"/>",true)</xsl:attribute>
					<xsl:value-of select="user:GetJudgeOptionText(string(select_answer_value))"  disable-output-escaping="yes"/>
				  </xsl:element>
                </xsl:element>
                <![CDATA[        ]]>
              </xsl:for-each>
            </xsl:when>
            
            <xsl:when test="question_base_type_code='judge_correct'">
              <!-- 判断题 -->
              <xsl:for-each select="select_answers/select_answer">
                <xsl:element name="input">
                  <xsl:attribute name="id">JudgeCorrect_<xsl:value-of select="../../question_uid"/></xsl:attribute>
                  <xsl:attribute name="name">JudgeCorrect_<xsl:value-of select="../../question_uid"/></xsl:attribute>
                  <xsl:attribute name="type">radio</xsl:attribute>
                  <xsl:attribute name="onclick">SetQuestionAnswerStatus("<xsl:value-of select="../../question_uid"/>",true)</xsl:attribute>
                  <xsl:attribute name="value"><xsl:value-of select="select_answer_value"/></xsl:attribute>
                  <xsl:element name="span">
                    <xsl:attribute name="id">spanSelectAnswer_<xsl:value-of select="../../question_uid"/></xsl:attribute>
                    <xsl:attribute name="onclick">jscomCheckedQuestionAnswer('Answer_<xsl:value-of select="../../question_uid"/>',<xsl:value-of select="position()-1" />);SetQuestionAnswerStatus("<xsl:value-of select="../../question_uid"/>",true)</xsl:attribute>
					<xsl:value-of select="user:GetJudgeOptionText(string(select_answer_value))"  disable-output-escaping="yes"/>
                  </xsl:element>
                </xsl:element>
                <![CDATA[        ]]>
              </xsl:for-each>
              <br />If it is wrong, please correct:
				<xsl:element name="TextArea">
                <xsl:attribute name="id">Answer_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="name">Answer_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="rows">5</xsl:attribute>
                <xsl:attribute name="style">width:100%</xsl:attribute>
                <xsl:attribute name="class">QuestionInputText</xsl:attribute>
                <xsl:attribute name="onchange">SetQuestionAnswerStatus("<xsl:value-of select="question_uid"/>",true)</xsl:attribute>
              </xsl:element>
            </xsl:when>
            
            <xsl:when test="question_base_type_code='answer'">
              <xsl:element name="input">
                <xsl:attribute name="type">hidden</xsl:attribute>
                <xsl:attribute name="id">hidSelectAnswer_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="name">hidSelectAnswer_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="value"><xsl:value-of select="select_answer"/></xsl:attribute>
              </xsl:element>
              <!-- 简答题 -->
			  <xsl:choose>
			  <xsl:when test="$exam_is_only_upload_file='Y' and is_answer_by_html='Y' and is_only_upload_file!='N'">
			  	<xsl:element name="TextArea">
					<xsl:attribute name="id">Answer_<xsl:value-of select="question_uid"/></xsl:attribute>
					<xsl:attribute name="name">Answer_<xsl:value-of select="question_uid"/></xsl:attribute>
					<xsl:attribute name="rows">5</xsl:attribute>
					<xsl:attribute name="style">width:100%;display:none;</xsl:attribute>
					<xsl:attribute name="class">QuestionInputText</xsl:attribute>
					<xsl:attribute name="onchange">SetQuestionAnswerStatus("<xsl:value-of select="question_uid"/>",true)</xsl:attribute>
				  </xsl:element>
				  <xsl:element name="input">
					<xsl:attribute name="type">button</xsl:attribute>
					<xsl:attribute name="value">Upload Attachment</xsl:attribute>
					<xsl:attribute name="id">btnUploadFile_<xsl:value-of select="question_uid"/></xsl:attribute>
					<xsl:attribute name="class">Button</xsl:attribute>
					<xsl:attribute name="onclick">OpenUploadFile('<xsl:value-of select="question_uid"/>')</xsl:attribute>
				  </xsl:element>
			  </xsl:when>
			  <xsl:otherwise>
				  <xsl:element name="TextArea">
					<xsl:attribute name="id">Answer_<xsl:value-of select="question_uid"/></xsl:attribute>
					<xsl:attribute name="name">Answer_<xsl:value-of select="question_uid"/></xsl:attribute>
					<xsl:attribute name="rows">5</xsl:attribute>
					<xsl:attribute name="style">width:100%</xsl:attribute>
					<xsl:attribute name="class">QuestionInputText</xsl:attribute>
					<xsl:attribute name="onchange">SetQuestionAnswerStatus("<xsl:value-of select="question_uid"/>",true)</xsl:attribute>
				  </xsl:element>
				  <xsl:if test="is_answer_by_html='Y'">
					  <xsl:element name="input">
						  <xsl:attribute name="type">button</xsl:attribute>
						  <xsl:attribute name="value">Edit</xsl:attribute>
						  <xsl:attribute name="id">
							  lnkAnswerByHtml_<xsl:value-of select="question_uid"/>
						  </xsl:attribute>
						  <xsl:attribute name="class">common_btn</xsl:attribute>
						  <xsl:attribute name="onclick">
							  javascript:OpenHtmlEditor('Answer_<xsl:value-of select="question_uid"/>','ExamAnswer/Exam_<xsl:value-of select="$exam_uid"/>/ExamGrade_<xsl:value-of select="$exam_grade_uid"/>/<xsl:value-of select="question_uid"/>')
						  </xsl:attribute>
					  </xsl:element>
				  </xsl:if>
			  </xsl:otherwise>
			  </xsl:choose>
            </xsl:when>

            <xsl:when test="question_base_type_code='operate'">
              <!-- 操作题 -->
              <xsl:element name="input">
                <xsl:attribute name="type">button</xsl:attribute>
                <xsl:attribute name="value">Begin</xsl:attribute>
                <xsl:attribute name="id">btnUsingOffice_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="class">Button</xsl:attribute>
                <xsl:attribute name="onclick">OpenContractTextFile('<xsl:value-of select="$exam_grade_uid"/>','<xsl:value-of select="question_uid"/>','<xsl:value-of select="$exam_uid"/>')</xsl:attribute>
              </xsl:element>
              <xsl:element name="input">
                <xsl:attribute name="id">Answer_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="name">Answer_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="type">hidden</xsl:attribute>
              </xsl:element>
              <xsl:element name="input">
                <xsl:attribute name="id">hidJudgeScore_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="name">hidJudgeScore_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="type">hidden</xsl:attribute>
              </xsl:element>
            </xsl:when>
			
			<xsl:when test="question_base_type_code='voice'">
              <!-- 语音题 -->
              <xsl:element name="input">
                <xsl:attribute name="type">button</xsl:attribute>
                <xsl:attribute name="value">Begin</xsl:attribute>
                <xsl:attribute name="id">btnUsingVoice_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="class">Button</xsl:attribute>
                <xsl:attribute name="onclick">OpenContractVoiceFile('<xsl:value-of select="$exam_grade_uid"/>','<xsl:value-of select="question_uid"/>','<xsl:value-of select="$exam_uid"/>')</xsl:attribute>
              </xsl:element>
              <xsl:element name="input">
                <xsl:attribute name="id">Answer_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="name">Answer_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="type">hidden</xsl:attribute>
              </xsl:element>
              <xsl:element name="input">
                <xsl:attribute name="id">hidJudgeScore_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="name">hidJudgeScore_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="type">hidden</xsl:attribute>
              </xsl:element>
            </xsl:when>
            
            <xsl:when test="question_base_type_code='typing'">
              <!-- 打字题 -->
              <xsl:element name="input">
                <xsl:attribute name="id">hidQuestionText_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="name">hidQuestionText_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="type">hidden</xsl:attribute>
                <xsl:attribute name="value"><xsl:value-of select="user:TrimQuoteForTypingQuestion(string(question_text))"/></xsl:attribute>
              </xsl:element>
              <xsl:element name="input">
                <xsl:attribute name="type">button</xsl:attribute>
                <xsl:attribute name="value">Begin</xsl:attribute>
                <xsl:attribute name="id">btnBeginTyping_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="class">Button</xsl:attribute>
                <xsl:attribute name="onclick">TypingOpenWindow('<xsl:value-of select="$exam_grade_uid"/>','<xsl:value-of select="question_uid"/>',<xsl:value-of select="paper_question_exam_time"/>,<xsl:value-of select="paper_question_score"/>)</xsl:attribute>
              </xsl:element>
              <xsl:element name="input">
                <xsl:attribute name="id">Answer_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="name">Answer_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="type">hidden</xsl:attribute>
              </xsl:element>
              <xsl:element name="input">
                <xsl:attribute name="id">hidJudgeScore_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="name">hidJudgeScore_<xsl:value-of select="question_uid"/></xsl:attribute>
                <xsl:attribute name="type">hidden</xsl:attribute>
              </xsl:element>
            </xsl:when>
          </xsl:choose>
        </TD>
      </TR>
  </xsl:template>
</xsl:stylesheet>