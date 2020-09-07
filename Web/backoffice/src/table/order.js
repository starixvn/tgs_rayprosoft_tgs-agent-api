export const Ascending = "ascending"

const AscendingNumber = 0

const AscendingBoolean = false

export const Descending = "descending"

const DescendingNumber = 1

const DescendingBoolean = true

export const of = (value) => {
  if (
    value === DescendingBoolean ||
    value === DescendingNumber ||
    isDescendingString(value)
  ) {
    return Descending
  }

  return Ascending
}

export const isAscending = (value) => of(value) === Ascending

export const equalsAscending = (value) => {
  return (
    value === AscendingNumber ||
    value === AscendingBoolean ||
    value === Ascending
  )
}

export const isDescending = (value) => of(value) === Descending

export const equalsDescending = (value) => {
  return (
    value === DescendingNumber ||
    value === DescendingBoolean ||
    value === Descending
  )
}

//Ascending => Descending => unset(null)
export const toggle = (value) => {
  if (equalsAscending(value)) {
    return Descending
  }

  if (equalsDescending(value)) {
    return null
  }

  return Ascending
}

const isString = (value) => typeof value === "string"

const isDescendingString = (value) =>
  isString(value) && value.toLowerCase() === Descending
