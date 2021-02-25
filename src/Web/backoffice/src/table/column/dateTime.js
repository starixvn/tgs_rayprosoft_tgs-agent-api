import React from "react"
import { css, Global } from "@emotion/react"
import { Text } from "bumbag"

import { formatDateTime } from "../../date/format"

const DefaultDateFormat = "yyyy/MM/dd"
const DefaultTimeFormat = "HH:mm:ss"

export const dateTimeColumn = ({
  Header,
  accessor,
  dateFormat = DefaultDateFormat,
  timeFormat = DefaultTimeFormat,
  ...props
}) => {
  return {
    Header,
    accessor,
    headCell: {
      textAlign: "center"
    },
    cell: {
      textAlign: "center"
    },
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
  }
}

export default dateTimeColumn
