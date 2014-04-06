var IntegrationConfig = {
    treegridbom: new Edo.lists.Table(),
    treegriddoc: new Edo.lists.Table(),
    fileoper : new FileOper(),
    startCheckDoc: function (curdata) {
        return Edo.util.JSON.encode(curdata);
    },
    startImportData: function (curdata) { },
    startUploadFile: function (curdata) {
        var allRes = true;
        var filecontrol = getFileOperate()();
        var dnc = filecontrol.getDotNetControler();
        if (dnc) {
            dnc.IsSilenceMode = curdata.length != 1;
        }
        if (curdata.length > 1) setProgress(curdata.length, 0);
        var _index = 0, fileoper = this.fileoper;
        Each(curdata, function (o) { //上传文档
            var filep = o["FilePath"], filen = o["FileName"];
            setProgressMessage(filen);
            if (filep && filen) {
                allRes &= filecontrol.UploadFile(filep + "\\" + filen, o.copyid);
                fileoper.DeleteFile(filep + "\\" + filen);
            }
            if (curdata.length > 1)
                setProgress(curdata.length, ++_index);
        });

        if (!allRes) return false; //上传失败
        return true;
    },
    dataBind: function (datasource) {
        var data = datasource.Data || [];
        this.treegriddoc.set('data', (function () {
            var res = [];
            var currow = null;
            for (var i = 0, l = data.length; i < l; ++i) {
                currow = data[i];
                if (currow && currow.filetype !== 'ADMAT') {
                    res.push(currow);
                }
            }
            return res;
        }())); //绑定文档数据
        this.treegridbom.set('data', data); //绑定BOM数据
        InitFloder(data);

        (function (grid) {
            if (!grid) return;
            var hidden = datasource.hidden;
            grid.data.filter(function (o) {
                if (hidden) { return o.filetype === 'ADMAT'; } //是PCB才有
                else if (o.OrderId == 0) return true; //最上层的文件
                return o.filetype === 'ADMAT'; //显示物料
            });
        }(this.treegridbom));
    },

    docAreaHeight: function () { return 200; },

    matAreaHeight: function () { return 250; },

    onResize: function () {
        var w = $(window).width() - 2;
        var h = $(window).height() - 330;
        if (h < 250) h = 250;
        if (w < 788) w = 788;
        this.treegriddoc.set('width', w);
        this.treegridbom.set('width', w);
        this.treegridbom.set('height', h);
        $("#BomInfo").height(h);
    },

    getDatasource: function () {
        return this.treegridbom.data.source;
    },
    setTableHeader: function (datasource) { //设置表头
        var docColumns = datasource.Doc || [];
        var bomColumns = datasource.BOM || [];
        SetTreeGrid(this.treegriddoc, "Documentinfo", docColumns, this.docAreaHeight());
        SetTreeGrid(this.treegridbom, "BomInfo", bomColumns, this.matAreaHeight());
    }
};

function CreateProduct(id) {
    ShowWinodw("../Material/CreateNewMaterial.aspx?mode=Product&allegro=1", "550px", "500px",
            function (value) { cbSetCellValue(value, id, "BomInfo"); });
    return false;
}

function CreateProductOrMaterial(id) {
    ShowWinodw("../Common/SelectMaterialOrProduct.aspx?Integration=1&UseType=New", "800px", "600px",
            function (value) { cbSetCellValue(value, id, "BomInfo"); });
    return false;
}

function SelectProductOrMaterial(id) {
    ShowWinodw("../Common/SelectMaterialOrProduct.aspx?Integration=1", "800px", "600px",
            function (value) { cbSetCellValue(value, id, "BomInfo"); });
    return false;
}
function SelectProduct(id, controlid) {//用于在多个相关对象中选择一个已有的产品
    var dg = getTreegrid(controlid);
    var record = dg.data.getById(id);
    var value = {};
    var matVerids = record.MaterialVerIds;
    ShowWinodw("../Common/MaterialBrowse.aspx?allegro=" + matVerids, "800px", "500px",
            function (value) { cbSetCellValue(value, id, controlid); });
    return false;
}

function getTreegrid(controlid) {
    if (controlid == "BomInfo") {
        return IntegrationConfig.treegridbom;
    }
    else if (controlid == "Documentinfo")
        return IntegrationConfig.treegriddoc;
}

function cbSetCellValue(value, id, controlid) {
    var dg = getTreegrid(controlid);
    var record = dg.data.getById(id);
    record.__matverid = value.VerId;
    record.MaterialCategoryName = value.CategoryName;
    record.MaterialName = value.Name;
    dg.data.update(record, 'MaterialCode', value.Code);
    Refurbish();
}

function ShowWinodw(url, w, h, callback) {
    var value = {};
    ShowWindowModal(url, w, h, value, value, { minimize: "yes", maximize: "yes" });
    value = window.returnValue;
    if (!value) return;
    callback(value);
}

function fileNameRender(value, record, column, rowIndex, data, t) {
    if (record.OrderId > 0) {
        return '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + value;
    }
    return value;
}

//"../Common/ProductSelectFrame.aspx"//用于选择一个已有的产品
function orderIdRender(value, record, column, rowIndex, data, t) {
    if (record.OrderId == 0) {
        var hasValue = !!value;
        var curvalue = null;
        if (!record.selectBOM) {
            var para = GetQueryString('Optype');
            if (para != "1" || (para == "1" && record.__ExistsBom == false)) {//导入 、检入时找不到bom
                curvalue = hasValue ? record.MaterialCode : '&nbsp;';
                return showButton(curvalue,
                        [{ funname: 'CreateProductOrMaterial', src: "../skins/New.png", txt: 'new', args: [record.__id] },
                         { funname: 'SelectProductOrMaterial', src: "../skins/Open.png", txt: 'select', args: [record.__id] }]);
            }
        }
        else {
            curvalue = hasValue ? record.MaterialCode : '&nbsp;';
            return showButton(curvalue,
                    [{ funname: 'SelectProduct', src: "../skins/Open.png", txt: 'select', args: [record.__id, 'BomInfo'] }]);
        }
    }
    return value;
}
function selectProductRender(value, record, column, rowIndex, data, t) {
    /*if (!value || value.toString().length == 0) {
    return showButton('&nbsp;', [{ funname: 'SelectProduct', src: "../skins/Open.png", txt: 'select', args: [record.__id, 'Documentinfo']}]);
    }
    else*/
    if (record.selectBOM) {
        return showButton(record.MaterialCode,
            [{ funname: 'SelectProduct', src: "../skins/Open.png", txt: 'select', args: [record.__id, 'Documentinfo'] }]);
    }
    return value;
}
/*
将funnames修改为 [{funname:"name", src: "../skins/Open.png", txt:"btntxt", args:[arg1,arg2]}]
将funname 修改为 {funname:"name" , src: "../skins/Open.png", txt:"btntxt", args:[arg1,arg2]}
*/
function showButton(showvalue, funnames) {
    var res = [];
    res.push('<div"> ');
    res.push('<div style="float:left;width:70%;overflow:hidden;white-space:nowrap;text-overflow:ellipsis;"');
    showvalue = showvalue || '&nbsp;';
    res.push(' title="');
    res.push(showvalue);
    res.push('" >');
    res.push(showvalue);
    res.push("</div>");
    res.push(' <div  style="float:right;">');

    for (var j = 0, len = funnames.length; j < len; ++j) {
        var funname = funnames[j].funname,
            args = funnames[j].args,
            src = funnames[j].src,
            txt = funnames[j].txt || "...";
        res.push('<img style="margin-top:5px; cursor:pointer;" alt="');
        res.push(txt);
        res.push('" src="');
        res.push(src);
        res.push('" onclick="javascript:');
        res.push(funname);
        res.push("(");
        for (var i = 0, l = args.length; i < l; ++i) {
            res.push("'" + args[i] + "'");
            if (i < l - 1) {
                res.push(',');
            }
        }
        res.push(');void(0);" value="');
        res.push(txt);
        res.push('" />');
    }

    res.push(' </div></div>');
    return res.join('');
}

function getintegrationPlug() {//获取服务器插件
    var Cadobj = null
    return function () {
        if (!Cadobj) {
            try {
                Cadobj = new ActiveXObject(IntegrationFactory()().integrationType);
                var apptype = GetQueryString("AppId");
                if (apptype == 'ALLEGROCIS') apptype = 'ZKSCH';
                if (apptype == 'ALLEGROPCB') apptype = 'ZKPCB';
                Cadobj.InitClass(apptype);
            } catch (e) {
                if (e.message) {
                    alert(e.message);
                } else {
                    alert("客户端组件丢失，重新安装或修复可解决此类问题。");
                }
                return null;
            }
        }
        return Cadobj;
    }
}

function bomtips() {
    if ('ALLEGROPCB' == GetQueryString("AppId")) return true;
    var data = IntegrationFactory()().getDatasource();
    if (data.length > 0) {
        for (var i = data.length - 1; i >= 1; i--) {
            if (data[i].filetype == 'ADMAT' && !data[i]['MaterialCode']) {
                return window.confirm('bom中存在没有编码的物料，是否继续生成bom？');
            }
        }
    }
    return true;
}

$(function () {
    var ddl = $('#ddlFileType');
    if (ddl.length > 0) {
        var txt = ddl.find('option:selected').text()
            , txtzk = '', container = $('#import_info > span');
        if (txt == 'orCAD') {
            txtzk = 'ZKSCH';
        } else {
            txtzk = 'ZKPCB';
        }
        ddl.replaceWith($('<span style="z-index:10; padding: 0 5px; border: 1px solid #ccc;">' + txtzk + '</span>'));
    }
})

function FileOper() {
    try {
        this.FSO = new ActiveXObject("Scripting.FileSystemObject");
    } catch (e) { }
}

FileOper.prototype.DeleteFile = function (path) {
    var fso = this.FSO;
    if (fso.FileExists(path)) {
        try {  
            fso.DeleteFile(path);
        } catch (e) { }
    }
}
