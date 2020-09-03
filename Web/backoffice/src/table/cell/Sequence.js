import React from "react"
import { Table } from "bumbag"

export const Head = (props) => {
  //TODO: i18n
  return <Table.HeadCell {...props}>序</Table.HeadCell>
}

export default ({ index, page, rows, ...props }) => (
  <Table.Cell {...props}>{(page - 1) * rows + (index + 1)}</Table.Cell>
)
