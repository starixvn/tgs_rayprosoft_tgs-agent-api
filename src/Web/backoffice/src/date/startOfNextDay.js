import { isValid } from "date-fns"
import addDays from "date-fns/addDays"
import parseISO from "date-fns/parseISO"
import startOfDay from "date-fns/startOfDay"

export const startOfNextDay = (value) => {
    let date = value
    if (typeof value === "string") {
        let parsed = parseISO(value)
        if (isValid(parsed)) {
            date = parsed
        }
    }

    if (isValid(date)) {
        let nextDay = addDays(value, 1)
        return startOfDay(nextDay)
    }

    return value
}
