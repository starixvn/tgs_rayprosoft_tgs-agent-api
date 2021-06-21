import { isValid } from "date-fns"
import subDays from "date-fns/subDays"
import parseISO from "date-fns/parseISO"
import startOfDay from "date-fns/startOfDay"

export const startOfPreviousDay = (value) => {
    let date = value
    if (typeof value === "string") {
        let parsed = parseISO(value)
        if (isValid(parsed)) {
            date = parsed
        }
    }

    if (isValid(date)) {
        let prevDay = subDays(value, 1)
        return startOfDay(prevDay)
    }

    return value
}
