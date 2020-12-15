import React from "react"
import { css, Global } from "@emotion/react"
import { Table, Set } from "bumbag"

export const Head = (props) => {
  //TODO: i18n
  return <Table.HeadCell {...props}>状态</Table.HeadCell>
}
