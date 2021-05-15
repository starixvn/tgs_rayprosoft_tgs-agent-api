export const stringifyJSON = (value = "") => {
    try {
        if (typeof value === "string") {
            let parsed = JSON.parse(value)
            return JSON.stringify(parsed, null, 4)
        }
    } catch {
        //ignore
    }

    return value
}

export const tryStringifyJSON = (value) => {
    try {
        if (typeof value === "string") {
            let parsed = JSON.parse(value)
            return [true, JSON.stringify(parsed, null, 4)]
        }
    } catch (error) {
        //ignore
        console.log("tryStringifyJSON error", error, "source", value)
    }

    return [false, value]
}

export const parseJSON = (value = "") => {
    let result = value
    try {
        if (typeof value === "string") {
            result = JSON.parse(value)
        }
    } catch {
        //ignore
    }
    return result
}
