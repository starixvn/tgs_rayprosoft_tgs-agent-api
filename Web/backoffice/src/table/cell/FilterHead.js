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
import { faCaretUp, faCaretDown } from "@fortawesome/free-solid-svg-icons"

export default ({ id = "", sort = "", order = "", children, ...props }) => (
  <Table.HeadCell {...props}>
    <div
      css={css`
        display: inline-flex;
        align-items: center;
      `}
    >
      {children} <SortIcon id={id} sort={sort} order={order} />
    </div>
  </Table.HeadCell>
)

//TODO: order type? 3 state?
const SortIcon = ({ id = "", sort = "", order = "" }) => {
  if (id != sort) {
    return null
  }

  return <Icon icon={faCaretUp} type="font-awesome"></Icon>
}
