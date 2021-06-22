export const stringifyJSON = (value = "") => {
    try {
        if (typeof value === "string" && value !== "") {
            let parsed = parseJSON(value)
            return JSON.stringify(parsed, null, 4)
        } else {
            return JSON.stringify(value, null, 4)
        }
    } catch (error) {
        //ignore
        console.log("stringifyJSON error source", value)
    }

    return value
}

export const tryStringifyJSON = (value) => {
    try {
        if (typeof value === "string") {
            let parsed = JSON.parse(value)
            return [true, JSON.stringify(parsed, null, 4)]
        } else {
            return [true, JSON.stringify(value, null, 4)]
        }
    } catch (error) {
        //ignore
        console.log("tryStringifyJSON error source", value)
    }

    return [false, value]
}

export const parseJSON = (value = "") => {
    let result = value
    try {
        if (typeof value === "string" && value !== "") {
            result = JSON.parse(value)
        }
    } catch (error) {
        //ignore
        console.log("parseJSON error source", value)
    }
    return result
}
