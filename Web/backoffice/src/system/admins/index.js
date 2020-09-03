import React, { useState, useEffect } from "react"
import { css, Global } from "@emotion/core"
import { useAsync } from "react-async"
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
import {
  Heading,
  PageContent,
  Stack,
  Table,
  Button,
  InputField,
  Set
} from "bumbag"
import client from "../../client"
import DataTable, { useTable } from "../../table"
import SequenceCell, {
  Head as SequenceHeadCell
} from "../../table/cell/Sequence"
import DateTimeCell from "../../table/cell/DateTime"
import { Head as StatusHeadCell } from "../../table/cell/Status"
import { Head as ActionHeadCell } from "../../table/cell/Action"

export const Code = "admins"

export const Content = () => {
  let table = useTable()
  let { page, setPage, rows, sortby, orderby, setPromise } = table
  let [username, setUsername] = useState("")
  let [trigger, setTrigger] = useState(false)

  const cleanup = () => {
    setTrigger(false)
  }

  const search = ({ username } = {}) => {
    setUsername(username)
    setPage(1)
    setTrigger(true)
  }

  useEffect(() => {
    let promise = getUsers({ page, rows, sortby, orderby, username })
    setPromise(promise)

    promise.then(cleanup)
  }, [page, rows, sortby, orderby, username, trigger])

  return (
    <PageContent css={css``}>
      <Stack orientation="vertical" spacing="major-2">
        <Heading use="h3">{title()}</Heading>
        <Stack orientation="horizontal">
          <SearchForm table={table} search={search} />
          <Button
            css={css`
              flex: 0;
            `}
          >
            TODO: 新增
          </Button>
        </Stack>
        <DataTable table={table}>
          {({ async, data = {}, ...props }) => {
            let { items = [] } = data

            //TODO: empty table body?
            //TODO: loading?
            return (
              <Table>
                <Table.Head>
                  <Table.Row>
                    <SequenceHeadCell />
                    <Account.HeadCell />
                    <Name.HeadCell />
                    <Department.HeadCell />
                    <PermissionGroup.HeadCell />
                    <Ip.HeadCell />
                    <LoggedAtHeadCell />
                    <CreatedAtHeadCell />
                    <StatusHeadCell />
                    <ActionHeadCell />
                  </Table.Row>
                </Table.Head>
                <Table.Body>
                  {items.map((row, index, list) => (
                    <Table.Row key={index}>
                      <SequenceCell index={index} page={page} rows={rows} />
                      <Account.Cell row={row} />
                      <Name.Cell row={row} />
                      <Department.Cell row={row} />
                      <PermissionGroup.Cell
                        row={row}
                        index={index}
                        list={list}
                      />
                      <Ip.Cell row={row} />
                      <DateTimeCell value={row.lastLoginTime} />
                      <DateTimeCell value={row.createdOn} />
                      <StatusCell row={row} />
                      <ActionCell row={row} />
                    </Table.Row>
                  ))}
                </Table.Body>
              </Table>
            )
          }}
        </DataTable>
      </Stack>
    </PageContent>
  )
}

//TODO: i18n
const Account = {
  HeadCell: () => <Table.HeadCell>用户名</Table.HeadCell>,
  Cell: ({ row }) => <Table.Cell>{row.username}</Table.Cell>
}
const Name = {
  HeadCell: () => <Table.HeadCell>姓名</Table.HeadCell>,
  Cell: ({ row }) => <Table.Cell>{row.name}</Table.Cell>
}
const Department = {
  HeadCell: () => <Table.HeadCell>部门</Table.HeadCell>,
  Cell: ({ row }) => <Table.Cell>{row.departmentName}</Table.Cell>
}
const PermissionGroup = {
  HeadCell: () => <Table.HeadCell>权限组</Table.HeadCell>,
  Cell: ({ row }) => <Table.Cell>{row.role}</Table.Cell>
}
const Ip = {
  HeadCell: () => <Table.HeadCell>最近登入IP</Table.HeadCell>,
  Cell: ({ row }) => <Table.Cell>{row.lastLoginIp}</Table.Cell>
}
const LoggedAtHeadCell = () => <Table.HeadCell>最近登入时间</Table.HeadCell>
const CreatedAtHeadCell = () => <Table.HeadCell>最近登入时间</Table.HeadCell>
//TODO: status mapping
const StatusCell = ({ row, index, list }) => (
  <Table.Cell>{row.status}</Table.Cell>
)
const ActionCell = ({ row, index, list }) => (
  <Table.Cell>
    <Set>
      <Button>TODO: 修改</Button>
      <Button>TODO: 修改密码</Button>
    </Set>
  </Table.Cell>
)

const SearchForm = ({ search, table, ...props }) => {
  let { promise } = table
  let { isPending } = useAsync({ promise })

  return (
    <Formik
      initialValues={{ username: "" }}
      onSubmit={(data) => {
        //let params = Object.entries(data)
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
