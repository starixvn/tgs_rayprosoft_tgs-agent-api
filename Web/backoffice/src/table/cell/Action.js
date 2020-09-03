import React from "react"
import { css, Global } from "@emotion/core"
import { Table, Set } from "bumbag"

export const Head = (props) => {
  //TODO: i18n
  return <Table.HeadCell {...props}>操作</Table.HeadCell>
}
