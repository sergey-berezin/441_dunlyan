
(function init () {
    let elem = document.getElementById('file1')
    elem.addEventListener('change', file_upload)
    load_data()
})()

async function file_upload(e) {
    let files = e.target.files
    for (let elem of files) {
        let prms = new Promise(res => {
            let reader = new FileReader()
            reader.onloadend = e => res(e.target.result)
            reader.readAsDataURL(elem)
        })
        let img = (await prms).split(',')[1]
        const post_request = {
            mode: 'cors',
            method: 'POST',
            body: JSON.stringify( { 
                "_ImageId": 0,
                "byte_image": img
            }),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
        }
        let response = await fetch('https://localhost:7285/api/Images', post_request)
        let emotionsString = await response.json()
        if (emotionsString != '') {
            let item_wrapper = document.createElement('div')
            item_wrapper.setAttribute('class', 'item_wrapper')
            item_wrapper.setAttribute('tabindex', '0')
            let grid = document.getElementById('grid')
            let new_item = document.createElement('img')
            new_item.setAttribute('src', 'data:image/png;base64,' + img)
            new_item.setAttribute('class', 'grid__item')
            new_item.emotions = emotionsString
            new_item.addEventListener('click', display_emotions)
            item_wrapper.appendChild(new_item)
            grid.appendChild(item_wrapper)
        }
    }
}
async function load_data(e) {
    let response = await fetch('https://localhost:7285/api/Images/AllImages')
    let images = await response.json()
    response = await fetch('https://localhost:7285/api/Images/AllResults')
    let emotions = await response.json()
    let grid = document.getElementById('grid')
    grid.innerHTML = ''
    for (let i = 0; i < images.length; i++) {
        let item_wrapper = document.createElement('div')
        item_wrapper.setAttribute('class', 'item_wrapper')
        item_wrapper.setAttribute('tabindex', '0')
        let grid_item = document.createElement('img')
        grid_item.setAttribute('src', 'data:image/png;base64,' + images[i])
        grid_item.setAttribute('class', 'grid__item')
        grid_item.emotions = emotions[i]
        grid_item.addEventListener('click', display_emotions)
        item_wrapper.appendChild(grid_item)
        grid.appendChild(item_wrapper)
    }
}

async function display_emotions(e) {
    let elem = document.getElementById('emotions')
    let emotions_string = e.currentTarget.emotions
    elem.innerHTML = ''
    let emotions_array = emotions_string.split('\n')
    emotions_array.forEach(element => {
        if (element != '') {
            let new_emotion = document.createElement('div')
            new_emotion.innerText = element.split(' ')[0] + ' - ' + element.split(' ')[1]
            elem.appendChild(new_emotion)
        }
    });
    
}

async function clear_Grid(e) {
    document.getElementById('grid').innerHTML = ''
    document.getElementById('emotions').innerHTML = ''
}

function button_File (event) {
    const file = document.getElementById('file1')
    file.click()
}