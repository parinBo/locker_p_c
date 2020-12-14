//default events given
var e= [];
$.get('https://localhost:5001/home/time', function (data) {
    layOutDay(data);
    data.forEach(element => console.log(element.start + "   " + element.end));
});

//const events = [{ start: 0, end: 90 }, { start: 90, end: 180 }, { start: 300, end: 360 }];

//layOutDay(events);

//function to generate mock events for testing

function generateMockEvents(n) {
    let events = [];
    let minutesInDay = 60 * 12;

    while (n > 0) {
        let start = Math.floor(Math.random() * minutesInDay)
        let end = start + Math.floor(Math.random() * (minutesInDay - start));
        events.push({ start: start, end: end })
        n--;
    }

    return events;
}