import parseISO from "date-fns/parseISO"
import isValid from "date-fns/isValid"
import dateOf from "date-fns/toDate"

export const toDate = (value) => {
  let date = undefined
  if (typeof value === "string") {
    date = parseISO(value)
  } else {
    date = dateOf(value)
  }

  if (isValid(date)) {
    return date
  }

  return undefined
}
