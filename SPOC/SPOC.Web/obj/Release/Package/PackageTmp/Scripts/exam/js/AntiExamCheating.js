
///////////////////////////////////////////////////////////////////////////////
//失去焦点3次自动提交答卷.用于TT、傲游浏览器及用选项卡打开的IE浏览器的判断
//现在暂时不用,现在处理TT和傲游浏览器的方法是：如果是闭卷考试，不能使用这两种浏览器考试
//注意，在引用此js的页面中，一定要包括如下隐藏控件
//<input id="hidIsOpenbookExam" style="width: 21px; height: 21px" type="hidden" size="1" name="hidIsOpenbookExam" runat="server">
//<input id="hidIsNormalOpenBrowser" style="width: 21px; height: 21px" type="hidden" size="1" name="hidIsNormalOpenBrowser" runat="server">
var errorFlagTimes=0;
var bolSubmmit=false;
var IsOpenbookExam="";
var IsNormalOpenBrowser="";
var illegalTimes=0;
document.onkeyup=function()
{
    if(IsOpenbookExam=="")
    {
        if(document.getElementById("hidIsOpenbookExam")!=null)
        {
            IsOpenbookExam=document.getElementById("hidIsOpenbookExam").value;
        }
    }
    if(IsNormalOpenBrowser=="")
    {
        if(document.getElementById("hidIsNormalOpenBrowser")!=null)
        {
            IsNormalOpenBrowser=document.getElementById("hidIsNormalOpenBrowser").value;
        }
    }
    //如果是开卷考试，或是练习，不用往下执行
    if(IsOpenbookExam=="Y"||IsOpenbookExam==""||IsNormalOpenBrowser==""||IsNormalOpenBrowser=="Y")
    {
        return ;
    }  
    
    if(event.altKey==true&&event.keyCode==9)
    {
         illegalTimes=illegalTimes+1;
         MessageBox_Show(Translate("AntiExam1","","非法操作！否则超过一次系统将自动提交试卷！"));
    }
    if(illegalTimes>1)
    {
        try
        {   
            if(!bolSubmmit)
            {   		                        
                window.opener.SubmitPaper();
                bolSubmmit=true;
            }
        }
        catch(e)
        {
            if(!bolSubmmit)
            {             
                window.opener.parent.SubmitPaper();
                bolSubmmit=true;
            }
        }
	    window.opener=null;
        window.close();
    }
}

document.onkeydown=function()
{
    if(IsOpenbookExam=="")
    {
        if(document.getElementById("hidIsOpenbookExam")!=null)
        {
            IsOpenbookExam=document.getElementById("hidIsOpenbookExam").value;
        }
    }
    if(IsNormalOpenBrowser=="")
    {
        if(document.getElementById("hidIsNormalOpenBrowser")!=null)
        {
            IsNormalOpenBrowser=document.getElementById("hidIsNormalOpenBrowser").value;
        }
    }
    //如果是开卷考试，或是练习，不用往下执行
    if(IsOpenbookExam==""||IsNormalOpenBrowser==""||IsOpenbookExam=="Y"||IsNormalOpenBrowser=="Y")
    {
        return ;
    }  
    if(event.altKey==true)
    {
        MessageBox_Show(Translate("AntiExam2","","警告!禁止使用Alt+Tab键切换窗口，否则答卷将自动提交！"));
    }
    //ctrl+86  ctrl+V 粘贴
	if(event.ctrlKey && event.keyCode == 86)
	{
			event.keyCode = 0;
			MessageBox_Show(Translate("AntiExam3","","不可以在考试中粘贴文本！"));
			event.returnValue = false;
			return;
	}
}

document.onfocusout = function(e)
{
    if(IsOpenbookExam=="")
    {
        if(document.getElementById("hidIsOpenbookExam")!=null)
        {
            IsOpenbookExam=document.getElementById("hidIsOpenbookExam").value;
        }
    }
    if(IsNormalOpenBrowser=="")
    {
        if(document.getElementById("hidIsNormalOpenBrowser")!=null)
        {
            IsNormalOpenBrowser=document.getElementById("hidIsNormalOpenBrowser").value;
        }
    }
    //如果是开卷考试，或是练习，不用往下执行
    if(IsOpenbookExam=="Y"||IsOpenbookExam==""||IsNormalOpenBrowser==""||IsNormalOpenBrowser=="Y")
    {
        return ;
    }  
    
    //获取当前页面的高度
    var height=document.body.clientHeight; 
//    var width=document.body.clientWidth;   
    try
    {
        if (document.layers) 
        { 
            var x=e.pageX; 
            var y=e.pageY;
        }
        if (document.all) 
        { 
            var x=event.clientX; 
            var y=event.clientY;
        }
        
        if(y<0||y>height)
        {        
            var times;
	        times = errorFlagTimes;
		    
            if(errorFlagTimes>=1)  //次数大于1 提交试卷.
            {		        
                try
                {   
                    if(!bolSubmmit)
                    {   		                        
	                    window.opener.SubmitPaper();
	                    bolSubmmit=true;
	                }
	            }
	            catch(e)
	            {
	                if(!bolSubmmit)
	                {
	                    window.opener.parent.SubmitPaper();
	                    bolSubmmit=true;
	                }
	            }
			    window.opener=null;
                window.close();
            }
            else
            {
	            times=times+1;
	            errorFlagTimes=times;
	            MessageBox_Show(Translate("AntiExam4","","请在试卷红色虚线区域内操作！否则超过一次系统将自动提交试卷！"));
            }
        }
    }
    catch(e)
    {		
    }	
}

function SetFormBorderCss(obj)
{
    if(IsOpenbookExam==""&&document.getElementById("hidIsOpenbookExam")!=null)
    {
        IsOpenbookExam=document.getElementById("hidIsOpenbookExam").value;
    }
    if(IsNormalOpenBrowser==""&&document.getElementById("hidIsNormalOpenBrowser")!=null)
    {
        IsNormalOpenBrowser=document.getElementById("hidIsNormalOpenBrowser").value;
    }
    if(IsOpenbookExam=="N"&&IsNormalOpenBrowser=="N")
    {
        obj.style.border="3px dashed red";
    }
}

function ClearFormBorderCss(obj)
{
    if(IsOpenbookExam==""&&document.getElementById("hidIsOpenbookExam")!=null)
    {
        IsOpenbookExam=document.getElementById("hidIsOpenbookExam").value;
    }
    if(IsNormalOpenBrowser==""&&document.getElementById("hidIsNormalOpenBrowser")!=null)
    {
        IsNormalOpenBrowser=document.getElementById("hidIsNormalOpenBrowser").value;
    }
    if(IsOpenbookExam=="N"&&IsNormalOpenBrowser=="N")
    {        
        obj.style.border="0px solid #000";
    }
}

/////////////////////////////////////////////////////////////////////