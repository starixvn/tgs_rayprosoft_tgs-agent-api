import React from "react"
import { css, Global } from "@emotion/react"
import { Text, Set } from "bumbag"

import { formatDateTime } from "../../date/format"
import textColumn from "./textColumn"

const DefaultDateFormat = "yyyy/MM/dd"
const DefaultTimeFormat = "HH:mm:ss"

export const dateTimeColumn = ({
    dateFormat = DefaultDateFormat,
    timeFormat = DefaultTimeFormat,
    ...props
}) =>
    textColumn({
        Cell: ({ value = "" } = {}) => {
            let formattedDate = dateFormat
                ? formatDateTime(dateFormat, value)
                : ""
            let formattedTime = timeFormat
                ? formatDateTime(timeFormat, value)
                : ""
            return (
                <Set orientation="vertical" alignX="center">
                    {formattedDate && (
                        <Text.Block fontSize={{ default: "14px" }}>
                            {formattedDate}
                        </Text.Block>
                    )}
                    {formattedTime && (
                        <Text.Block fontSize={{ default: "14px" }}>
                            {formattedTime}
                        </Text.Block>
                    )}
                </Set>
            )
        },
        ...props
    })

export default dateTimeColumn
