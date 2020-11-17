import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field } from "formik"
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
  Checkbox
} from "bumbag"
import { useTable, Table, pageCount } from "../../table"
import dateTimeColumn from "../../table/column/dateTime"
import { useOperations } from "./client"
import { Trans } from "../../i18n"
import format from "../../date/format"
import { pastWeek } from "../../date/range"
import DateTimeRangeInput from "../../date/RangeInput"
import SearchButton from "../../button/Search"
import { Tab as _Tab, TabPanel } from "../../tab"

export const Key = "operation"

export const Tab = ({ ...props }) => {
  return (
    <_Tab tabId={Key} {...props}>
      <Trans zh="操作记录" en="Operating records" />
    </_Tab>
  )
}

export const Panel = ({ playerCode, ...props }) => {
  let { selectedId } = props
  let isActive = selectedId === Key
  let dateRange = pastWeek()
  let { state, setState, setPageIndex, setSortOrder } = useTable({
    from: dateRange.start,
    to: dateRange.end
  })
  let { pageIndex, pageSize, sortby, orderby, from, to } = state
  let { data = {}, isLoading, mutate } = useOperations(playerCode, {
    page: pageIndex + 1,
    rows: pageSize,
    sortby,
    orderby,
    from: format(from),
    to: format(to)
  })
  let columns = useColumns()

  return (
    <TabPanel tabId={Key} {...props}>
      <Stack spacing="major-2">
        <SearchBar
          start={from}
          end={to}
          search={({ start, end }) => {
            if (from === start && to === end) {
              mutate()
            }
            setState({
              from: start,
              to: end
            })
          }}
          isLoading={isLoading}
        />
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
    </TabPanel>
  )
}

const SearchBar = ({
  start: startDate,
  end: endDate,
  search,
  isLoading,
  ...props
}) => {
  let [start, setStart] = useState(startDate)
  let [end, setEnd] = useState(endDate)

  return (
    <Set>
      <DateTimeRangeInput
        start={start}
        end={end}
        validate
        onChangeStart={(date) => {
          setStart(date)
        }}
        onChangeEnd={(date) => {
          setEnd(date)
        }}
      />
      <SearchButton
        isLoading={isLoading}
        onClick={() => {
          search({ start, end })
        }}
      />
    </Set>
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
