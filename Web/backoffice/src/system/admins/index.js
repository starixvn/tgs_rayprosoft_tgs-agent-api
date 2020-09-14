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
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import AsyncTable, { useTableState } from "../../table/async"

export const Code = "admins"

//TODO: i18n
export const label = (code = "") => {
  code = code.toLowerCase()
  switch (code) {
    case "create":
      return "新增管理员"
    case "update":
      return "编辑管理员"
  }
}

export const Content = () => {
  let columns = useMemo(tableColumns)
  let { state, setState, headSortBy, headOrderBy } = useTableState({
    username: ""
  })
  let { pageIndex, pageSize, username = "", sortBy = [], request } = state

  let async = useAsync({ promise: request })
  let { isPending } = async

  useEffect(() => {
    let promise = getUsers({
      page: pageIndex + 1,
      rows: pageSize,
      username,
      sortby: headSortBy(),
      orderby: headOrderBy()
    })
    setState({ ...state, request: promise })
  }, [pageIndex, pageSize, username, sortBy])

  return (
    <PageContent
      title={title()}
      actionBar={
        <>
          <SearchForm search={setState} isPending={isPending} />
          <AddAdminButton />
        </>
      }
    >
      <AsyncTable
        state={state}
        onStateChange={setState}
        promise={request}
        head
        body
        columns={columns}
      />
    </PageContent>
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
