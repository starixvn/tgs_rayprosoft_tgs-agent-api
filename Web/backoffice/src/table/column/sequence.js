import React from "react"
import { Table } from "bumbag"

export default () => {
  //TODO: Header i18n
  return {
    accessor: (row, rowIndex) => {
      return {
        rowIndex
      }
    },
    Header: "序",
    width: 50,
    Cell: ({ state = {}, value = {} } = {}) => {
      let { pageIndex, pageSize } = state
      let { rowIndex } = value
      let row = rowIndex + 1
      let sequence = pageIndex * pageSize + row
      return sequence
    }
  }
}

/*
export const Head = (props) => {
  //TODO: i18n
  return <Table.HeadCell {...props}>序</Table.HeadCell>
}

export default ({ index, page, rows, ...props }) => (
  <Table.Cell {...props}>{(page - 1) * rows + (index + 1)}</Table.Cell>
)
*/
