const containerHeight = 720;
const containerWidth = 600;
const minutesinDay = 60 * 12;
let collisions = [];
let width = [];
let leftOffSet = [];
let id = "";
var c = 0;
let countt = 0;
let old = "";
var url = "https://localhost:5004/"
//var url1 = "https://10.153.139.170:5004/"
//var url2 = "https://localhost:44369/"
//var url3 = "https://192.168.2.52:5004/"
$(document).ready(function () {
    $("path").click(function () {
        id = this.id;
        $("#input_locker").attr("value", id);
        $("#input_locker").attr("readonly", "readonly");
        $("td").remove()
        main()
        
    });
})

var createtable = (s, e, n) => {
    var table = document.getElementById("tablebody");
    var tr = document.createElement("tr");
    var td1 = document.createElement("td");
    var td2 = document.createElement("td");
    var td3 = document.createElement("td");
    td1.innerHTML = s;
    td2.innerHTML = e;
    td3.innerHTML = n;
    tr.appendChild(td1);
    tr.appendChild(td2);
    tr.appendChild(td3);
    table.appendChild(tr);

}
var tablesss = async (s) => {
    s.forEach((e) => {
        let start = e.start
        let end = e.end
        let name = e.name
        createtable(start,end,name)
       
    })
    c=0
    
}

var globalData;
const dataaa = async (id) => {
    try {
        var urlParams = new URLSearchParams(window.location.search);
        var date = urlParams.get('date')
        if (date == null) {
            date = moment().format('YYYY-MM-DD')
        }
        const data = await fetch('home/time/' + id + "?date="+date);
        globalData = data.json();
        return globalData;
    }
    catch (err) {
        console.log(err);
    }
};

const main = async () => {
    let a = await dataaa(id);
    await tablesss(a)
 
}
