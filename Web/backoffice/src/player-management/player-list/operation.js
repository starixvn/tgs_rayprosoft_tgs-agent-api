import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import { Formik, Field, Form, useFormikContext } from "formik"
import {
  useTabState,
  Tab as _Tab,
  TabList,
  TabPanel as _TabPanel
} from "reakit/Tab"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  Label,
  Input,
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Tabs as _Tabs,
  Dialog,
  Modal,
  useClassName
} from "bumbag"
import { useTable, Table, pageCount } from "../../table"
import dateTimeColumn from "../../table/column/dateTime"
import { useOperations } from "./client"

export const Key = "operation"

export const Tab = ({ ...props }) => {
  let tabProps = _Tabs.Tab.useProps()
  return (
    <_Tab {...tabProps} {...props} id={Key}>
      {title()}
    </_Tab>
  )
}

export const Panel = ({ playerCode, ...props }) => {
  //TODO: default start & end time
  let { state, setPageIndex, setSortOrder } = useTable({
    start,
    end
  })
  let { pageIndex, pageSize, sortby, orderby, start, end } = state
  let { data = {}, isLoading } = useOperations({
    playerCode,
    searchParams: {
      page: pageIndex + 1,
      rows: pageSize,
      sortby,
      orderby
    }
  })
  let columns = useColumns()

  return (
    <_TabPanel {...props} id={Key}>
      <Table
        columns={columns}
        data={data?.items}
        pageCount={pageCount(pageSize, data?.totalCount)}
        setPageIndex={setPageIndex}
        setSortOrder={setSortOrder}
        initialState={state}
        isLoading={isLoading}
      />
    </_TabPanel>
  )
}

//TODO: i18n
const title = () => "操作记录"

const useColumns = () =>
  useMemo(() => [
    dateTimeColumn({
      Header: "操作时间",
      accessor: "lastLoginTime",
      disableSortBy: true
    }),
    {
      Header: "操作项",
      accessor: "operation",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    },
    {
      Header: "操作者",
      accessor: "operator",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    }
  ])
