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
