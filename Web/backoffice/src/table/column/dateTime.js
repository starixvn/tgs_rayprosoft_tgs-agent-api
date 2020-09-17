import React from "react"
import { css, Global } from "@emotion/core"
import { Table, Set, Text } from "bumbag"
import dayjs from "dayjs"

const DefaultDateFormat = "YYYY/MM/DD"
const DefaultTimeFormat = "HH:mm:ss"

export default ({
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
      let date = dayjs(value)
      if (!date.isValid()) {
        //empty
        return ""
      }

      let dateString = dateFormat ? date.format(dateFormat) : ""
      let timeString = timeFormat ? date.format(timeFormat) : ""

      return (
        <Text.Block
          css={css`
            white-space: nowrap;
          `}
        >
          {dateString} {timeString}
        </Text.Block>
      )
    },
    ...props
  }
}
