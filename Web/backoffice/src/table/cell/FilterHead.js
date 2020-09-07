import React, { useState, useEffect } from "react"
import { css, Global } from "@emotion/core"
import {
  Heading,
  Box,
  PageContent,
  Container,
  Stack,
  Icon,
  Table,
  Pagination,
  Button,
  InputField,
  Set
} from "bumbag"
import { isDescending, toggle, Ascending } from "../order"
import { faCaretUp, faCaretDown } from "@fortawesome/free-solid-svg-icons"

export default ({ id = "", table = {}, children, ...props }) => (
  <Table.HeadCell
    onClick={() => {
      let value = Ascending
      if (table.sortby == id) {
        value = toggle(table.orderby)
        if (value === null) {
          //clear sort
          id = ""
        }
      }
      table.setSortOrder({ sortby: id, orderby: value })
    }}
    css={css`
      cursor: pointer;
    `}
    {...props}
  >
    <div
      css={css`
        display: inline-flex;
        align-items: center;
      `}
    >
      {children} <SortIcon id={id} table={table} />
    </div>
  </Table.HeadCell>
)

const SortIcon = ({ id = "", table = {} }) => {
  if (id != table.sortby) {
    return null
  }

  return (
    <Icon
      icon={isDescending(table.orderby) ? faCaretDown : faCaretUp}
      type="font-awesome"
    ></Icon>
  )
}
