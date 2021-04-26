export const Ascending = "ascending"

const AscendingNumber = 0

const AscendingBoolean = false

export const Descending = "descending"

const DescendingNumber = 1

const DescendingBoolean = true

export const SortOrder = {
    Ascending,
    Descending
}

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

const isString = (value) => typeof value === "string"

const isDescendingString = (value) =>
    isString(value) && value.toLowerCase() === Descending

export const fromIsDescending = (isDescending) => {
    if (isDescending == null || isDescending === undefined) {
        return ""
    }

    return isDescending ? Descending : Ascending
}
