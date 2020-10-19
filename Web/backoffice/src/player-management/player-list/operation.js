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
import { Trans } from "../../i18n"

export const Key = "operation"

export const Tab = ({ ...props }) => {
  let tabProps = _Tabs.Tab.useProps()
  return (
    <_Tab {...tabProps} {...props} id={Key}>
      <Trans zh="操作记录" en="Operating records" />
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
      <Stack margin="major-1">
        <Box>TODO: date range input</Box>
        <Table
          columns={columns}
          data={data?.items}
          pageCount={pageCount(pageSize, data?.totalCount)}
          setPageIndex={setPageIndex}
          setSortOrder={setSortOrder}
          initialState={state}
          isLoading={isLoading}
        />
      </Stack>
    </_TabPanel>
  )
}

const useColumns = () =>
  useMemo(() => [
    dateTimeColumn({
      Header: <Trans zh="操作时间" en="Operation Time" />,
      accessor: "createdOn",
      disableSortBy: true
    }),
    {
      Header: <Trans zh="操作项" en="Operation Item" />,
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
      Header: <Trans zh="操作者" en="Operator" />,
      accessor: "createdByUsername",
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
