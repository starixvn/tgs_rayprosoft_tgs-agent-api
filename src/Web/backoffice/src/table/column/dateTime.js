import React from "react"
import { css, Global } from "@emotion/react"
import { Text } from "bumbag"

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
            return (
                <Text.Block
                    css={css`
                        white-space: nowrap;
                    `}
                >
                    {`${dateFormat ? formatDateTime(dateFormat, value) : ""} ${
                        timeFormat ? formatDateTime(timeFormat, value) : ""
                    }`}
                </Text.Block>
            )
        },
        ...props
    })

export default dateTimeColumn
