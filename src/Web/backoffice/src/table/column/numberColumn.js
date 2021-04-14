import React from "react"
import { css, Global } from "@emotion/react"
import { Text } from "bumbag"
import { FormattedNumber } from "../../i18n"
import { sumColumn, sumColumnValue } from "../sumColumn"

const DefaultNumberFormat = "0,0"

const defaultValueFn = (value) => value

export const numberColumn = ({
    Cell,
    format = DefaultNumberFormat,
    postfix = "",
    sumColumn: sumColumnName = "",
    toValue = defaultValueFn,
    ...props
}) => {
    const DefaultCell = ({ value }) => (
        <FormattedNumber value={value} format={format} postfix={postfix} />
    )
    Cell = Cell ?? DefaultCell
    let basic = {
        Cell,
        headCell: {
            textAlign: "center",
            whiteSpace: "nowrap"
        },
        cell: {
            textAlign: "center",
            whiteSpace: "nowrap"
        }
    }

    if (sumColumnName) {
        basic = {
            ...basic,
            Footer: (table) => {
                return (
                    <FormattedNumber
                        value={sumColumnValue(
                            sumColumnName,
                            toValue,
                            table.rows
                        )}
                        format={format}
                    />
                )
            },
            footerCellProps: {
                textAlign: "center"
            }
        }
    }

    return {
        ...basic,
        ...props
    }
}

const DefaultNumberDecimal = "0,0.00"
export const numberDecimalColumn = ({
    format = DefaultNumberDecimal,
    ...props
}) => numberColumn({ format, ...props })

export const coloredNumberDecimalColumn = ({
    value,
    format = DefaultNumberDecimal,
    postfix,
    ...props
}) =>
    numberDecimalColumn({
        format,
        Cell: ({ value }) => {
            let isPositive = value >= 0
            return (
                <Text color={isPositive ? "success" : "danger"}>
                    <FormattedNumber
                        value={value}
                        format={format}
                        postfix={postfix}
                    />
                </Text>
            )
        },
        ...props
    })

export const numberDecimalPercentageColumn = ({
    format = DefaultNumberDecimal,
    postfix = "%",
    ...props
}) => numberColumn({ format, postfix, ...props })

export default numberColumn
