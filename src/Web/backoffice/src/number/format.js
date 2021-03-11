import numeral from "numeral"

export const format = (format, value) => {
    if (Number.isFinite(value)) {
        return numeral(value).format(format)
    }

    return ""
}

export const formatNumber = (value) => format("0,0", value)

export const formatNumberDecimal = (value) => format("0,0.00", value)
