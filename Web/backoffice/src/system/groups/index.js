import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/core"
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Link,
  Redirect,
  useHistory,
  useLocation,
  useRouteMatch
} from "react-router-dom"
import client from "../../client"
import Async, { useAsync } from "react-async"
import { Formik, Field } from "formik"
import { Box, Stack, Table, Button, InputField, Set, Pagination } from "bumbag"
import PageContent from "../../page/Content"
import AsyncTable, { useAsyncTable } from "../../table/async"
import sequenceColumn from "../../table/column/sequence"

export const Code = "groups"

const defaultState = {
  pageIndex: 0,
  pageSize: 10,
  username: ""
}

export const Content = ({}) => {
  let [state = {}, setState] = useState(defaultState)
  let { pageIndex, pageSize, username = "" } = state

  const onStateChange = (state) => {
    setState((oldState) => ({
      ...oldState,
      ...state
    }))
  }

  return (
    <Async
      deferFn={() =>
        getUsers({ page: pageIndex + 1, rows: pageSize, username })
      }
    >
      {({ run: fetchData, promise, isPending }) => {
        return (
          <PageContent
            title={title()}
            actionBar={
              <>
                <SearchForm
                  search={(data) => {
                    onStateChange(data)
                    fetchData()
                  }}
                  isPending={isPending}
                />
                <Button
                  css={css`
                    flex: 0;
                  `}
                >
                  TODO: 新增
                </Button>
              </>
            }
          >
            <AsyncGroupTable
              state={state}
              onStateChange={onStateChange}
              fetchData={fetchData}
              promise={promise}
            />
          </PageContent>
        )
      }}
    </Async>
  )
}

const AsyncGroupTable = ({ state, onStateChange, fetchData, promise }) => {
  let columns = useMemo(tableColumns)

  return (
    <AsyncTable
      state={state}
      onStateChange={onStateChange}
      fetchData={fetchData}
      promise={promise}
      columns={columns}
    >
      <TableHead />
      <TableBody />
    </AsyncTable>
  )
}

const TableHead = ({}) => {
  let { table, async } = useAsyncTable()
  let { isPending } = async
  let { headerGroups } = table
  return (
    <Table.Head>
      {headerGroups.map((headerGroup) => (
        <Table.Row {...headerGroup.getHeaderGroupProps()}>
          {headerGroup.headers.map((column) => (
            <Table.HeadCell {...column.getHeaderProps()}>
              {column.render("Header")}
            </Table.HeadCell>
          ))}
        </Table.Row>
      ))}
    </Table.Head>
  )
}

const TableBody = ({}) => {
  let { table, async } = useAsyncTable()
  let { isPending } = async
  let { headerGroups, rows, getTableBodyProps, prepareRow } = table

  return (
    <Table.Body {...getTableBodyProps()}>
      {rows.map((row, i) => {
        prepareRow(row)
        return (
          <Table.Row {...row.getRowProps()}>
            {row.cells.map((cell) => {
              return (
                <Table.Cell {...cell.getCellProps()}>
                  {cell.render("Cell")}
                </Table.Cell>
              )
            })}
          </Table.Row>
        )
      })}
    </Table.Body>
  )
}

const SearchForm = ({ search, isPending, ...props }) => {
  return (
    <Formik
      initialValues={{ username: "" }}
      onSubmit={(data) => {
        search(data)
      }}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <Field
              component={InputField.Formik}
              name="username"
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("username", "")}>
                  X
                </Button>
              }
              placeholder={searchInputPlaceHolder()}
              autocomplete="off"
            />
            <Button type="submit" isLoading={isPending}>
              {searchButtonLabel()}
            </Button>
          </Set>
        </form>
      )}
    </Formik>
  )
}

const getUsers = (searchParams = {}) => {
  return client.get("/api/users", { searchParams })
}

//TODO: i18n
const title = () => "权限组管理"

//TODO: i18n
const searchButtonLabel = () => "搜索"

//TODO: i18n
const searchInputPlaceHolder = () => "权限组名称输入框"
const tableColumns = () => [
  sequenceColumn(),
  {
    Header: "权限组名称",
    accessor: "name"
  },
  {
    Header: "操作",
    Cell: () => {
      return (
        <Set>
          <Button>TODO: 修改</Button>
          <Button>TODO: 复制</Button>
          <Button>TODO: 删除</Button>
        </Set>
      )
    }
  }
]
