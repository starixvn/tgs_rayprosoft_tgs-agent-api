import React from "react"
import { css, Global } from "@emotion/core"
import { Table, Set } from "bumbag"
import dayjs from "dayjs"

const DefaultDateFormat = "YYYY/MM/DD"
const DefaultTimeFormat = "HH/mm/ss"

export default ({
  Header,
  accessor,
  dateFormat = DefaultDateFormat,
  timeFormat = DefaultTimeFormat
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

      return `${dateString} ${timeString}`
    }
  }
}

/*
export default ({
  value,
  dateFormat = DefaultDateFormat,
  timeFormat = DefaultTimeFormat,
  ...props
}) => {
  let date = dayjs(value)
  if (!date.isValid()) {
    //empty
    return <Table.Cell {...props}></Table.Cell>
  }

  return (
    <Table.Cell css={css``} {...props}>
      {`${date.format(dateFormat)} ${date.format(timeFormat)}`}
    </Table.Cell>
  )
}
*/
