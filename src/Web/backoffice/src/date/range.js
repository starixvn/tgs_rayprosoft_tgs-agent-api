import startOfYesterday from "date-fns/startOfYesterday"
import endOfYesterday from "date-fns/endOfYesterday"
import subWeeks from "date-fns/fp/subWeeks"
import startOfToday from "date-fns/startOfToday"
import endOfToday from "date-fns/endOfToday"

export const yesterday = () => {
    return {
        start: startOfYesterday(),
        end: endOfYesterday()
    }
}

export const today = () => {
    return {
        start: startOfToday(),
        end: endOfToday()
    }
}

export const pastWeek = () => {
    let end = new Date()
    let start = subWeeks(1, end)

    return {
        start,
        end
    }
}
