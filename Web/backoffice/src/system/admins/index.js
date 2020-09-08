import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import client from "../../client"
import Async, { useAsync } from "react-async"
import { Formik, Field } from "formik"
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
import { Heading, Stack, Table, Button, InputField, Set } from "bumbag"
import PageContent from "../../page/Content"
import AddAdminButton from "./add"
import AsyncTable, { useAsyncTable } from "../../table/async"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
//TODO: move to table.js?
import { faCaretUp, faCaretDown } from "@fortawesome/free-solid-svg-icons"

export const Code = "admins"

const defaultState = {
  pageIndex: 0,
  pageSize: 10,
  username: ""
}

export const Content = () => {
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
                <AddAdminButton />
              </>
            }
          >
            <AsyncAdminTable
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

const AsyncAdminTable = ({ state, onStateChange, fetchData, promise }) => {
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
              <div
                css={css`
                  display: inline-flex;
                  align-items: center;
                `}
              >
                {column.render("Header")}
                {column.isSorted ? (
                  column.isSortedDesc ? (
                    <Icon icon={faCaretDown} type="font-awesome" />
                  ) : (
                    <Icon icon={faCaretUp} type="font-awesome" />
                  )
                ) : (
                  ""
                )}
              </div>
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
const searchButtonLabel = () => "搜索"

//TODO: i18n
const searchInputPlaceHolder = () => "管理员用户名输入框"

//TODO: i18n
const title = () => "管理员列表"

const tableColumns = () => [
  sequenceColumn(),
  {
    Header: "用户名",
    accessor: "username",
    canSort: true
  },
  {
    Header: "姓名",
    accessor: "name"
  },
  {
    Header: "部门",
    accessor: "departmentName"
  },
  {
    Header: "权限组",
    accessor: "role"
  },
  {
    Header: "最近登入IP",
    accessor: "lastLoginIp"
  },
  dateTimeColumn({
    Header: "最近登入时间",
    accessor: "lastLoginTime"
  }),
  dateTimeColumn({
    Header: "创建时间",
    accessor: "createdOn"
  }),
  {
    Header: "状态",
    accessor: "status"
  },
  {
    Header: "操作",
    Cell: () => {
      return (
        <Set>
          <Button>TODO: 修改</Button>
          <Button>TODO: 修改密码</Button>
        </Set>
      )
    }
  }
]
