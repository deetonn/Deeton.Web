let root = document.getElementById("main");
const rootRoot = root

const rootDiv = document.createElement('div');
rootDiv.id = 'core-div'
// Forget previous root
root = rootDiv

let mono = {
    loaded: false,
    flags: {
        hydrated: {
            eps: false,
            motd: false
        }
    },
    tasks: [],
};

fetch("/dashboard/motd")
    .then(x => x.json()
        .then(x => {
            mono.motd = x.message
        }))
    .catch(e => console.log(`failed to fetch motd: ${e}`))

fetch("/_deeton/endpoints")
    .then(x => x.json()
        .then(x => {
            mono.endpoints = x.data
        }))
    .catch(e => console.error(`ERROR: failed to get endpoints - ${e}`))

const motd = document.createElement('h2')
motd.id = 'app-motd'
motd.innerText = mono.motd;

root.appendChild(motd)

const endpointsBox = document.createElement('div')
endpointsBox.id = "endpoints"
endpointsBox.style.display = "flex"
endpointsBox.style.flexDirection = 'column'
endpointsBox.style.justifyItems = 'center'
endpointsBox.style.flexWrap = 'wrap'

const executionBox = document.createElement('div')
executionBox.id = 'execution'

root.appendChild(endpointsBox)
root.appendChild(executionBox)

rootRoot.appendChild(root)

setInterval(() => {
    react();

    if (!mono.loaded) {
        clean()
        mono.loaded = true
    }
}, 1000)

const react = () => {
    if (mono.motd !== undefined
        && mono.flags.hydrated.motd != true
    ) {
        motd.innerHTML = mono.motd
        mono.flags.hydrated.motd = true
    }

    if (
        mono.endpoints !== undefined 
        && mono.endpoints.length !== 0
        && mono.flags.hydrated.eps != true) 
    {
        console.log("loading endpoint information for display.")
        let i = 0;
        for (let endpoint of mono.endpoints) {
            endpointsBox.appendChild(endpointBox(endpoint, i++))
        }
        mono.flags.hydrated.eps = true
    }

    if (mono.tasks.length !== 0) {
        execute_tasks(mono.tasks)
    }
}

const execute_tasks = (tasks) => {
    const executionBox = document.getElementById('execution')
    if (tasks.action == 'fetch') {
        let result = null;
        fetch(task)
    }
}

const simpleText = (text) => {
    let p = document.createElement('p')
    p.innerHTML = text
    return p
}

const clean = () => {
    // At this point we have fully loaded and just react to changes.
    // We remove the loading <p> tag.

    const loading = document.getElementById('loading')
    loading.remove();
}

const endpointBox = (data, id) => {
    const epText = data.path
    const contentType = data.contentType;

    const div = document.createElement('div')
    div.id = `endpoint-${id}`
    div.style.display = 'flex'
    div.style.justifyContent = 'left'
    div.style.maxHeight = '50%'
    div.style.maxWidth = '50%'
    div.style.border = 'solid'

    const epPathNode = document.createElement('p')
    const contentTypeNode = document.createElement('p')

    epPathNode.innerHTML = `URL: ${epText}`
    epPathNode.style.border = 'solid'
    epPathNode.style.margin = '5px'

    contentTypeNode.innerHTML = `Content-Type: ${contentType}`
    contentTypeNode.style.border = 'solid'
    contentTypeNode.style.margin = '5px'

    div.appendChild(epPathNode);
    div.appendChild(contentTypeNode);

    const executeButton = document.createElement('button')
    executeButton.onclick = (event) => {
        const thisEndpoint = mono.endpoints[id]
        mono.tasks.push({
            action: 'fetch',
            info: thisEndpoint
        })
    }

    return div
}