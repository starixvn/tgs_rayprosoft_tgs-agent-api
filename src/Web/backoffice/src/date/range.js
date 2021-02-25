import startOfYesterday from "date-fns/startOfYesterday"
import endOfYesterday from "date-fns/endOfYesterday"
import subWeeks from "date-fns/fp/subWeeks"

export const yesterday = () => {
  return {
    start: startOfYesterday(),
    end: endOfYesterday()
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
