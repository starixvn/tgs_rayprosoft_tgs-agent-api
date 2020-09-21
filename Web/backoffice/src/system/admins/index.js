import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import client from "../../client"
import Async, { useAsync } from "react-async"
import { Formik, Field } from "formik"
import { useLocation, useRouteMatch } from "react-router-dom"
import { Heading, Stack, Table, Button, InputField, Set } from "bumbag"
import PageContent from "../../page/Content"
import { AddButton, EditButton, EditPasswordButton } from "./edit"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import AsyncTable, { useTableState } from "../../table/async"
import { label as statusLabel } from "./status"
import { useIsDisplayMenu } from "../../user/context"

export const Code = "admins"

export const Content = () => {
  let { state, setState, headSortBy, headOrderBy } = useTableState({
    username: ""
  })
  let { pageIndex, pageSize, username = "", sortBy = [], request } = state

  let fetchTable = () => {
    let promise = getUsers({
      page: pageIndex + 1,
      rows: pageSize,
      username,
      sortby: headSortBy(),
      orderby: headOrderBy()
    })
    setState({ ...state, request: promise })
  }

  let async = useAsync({ promise: request })
  let { isPending } = async

  let { path = "" } = useRouteMatch()
  let [_, ...codes] = path.split("/")
  let isDisplay = useIsDisplayMenu(codes)

  useEffect(() => {
    if (isDisplay) {
      fetchTable()
    }
  }, [pageIndex, pageSize, username, sortBy])

  let columns = useMemo(() =>
    tableColumns({
      fetchTable
    })
  )

  if (!isDisplay) {
    return null
  }

  return (
    <PageContent
      title={title()}
      actionBar={
        <>
          <SearchForm search={setState} isPending={isPending} />
          <AddButton onSubmitted={fetchTable} />
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

const tableColumns = ({ fetchTable }) => [
  sequenceColumn(),
  {
    Header: "用户名",
    accessor: "username",
    headCell: {
      textAlign: "center"
    },
    cell: {
      textAlign: "center"
    }
  },
  {
    Header: "姓名",
    accessor: "name",
    disableSortBy: true,
    headCell: {
      textAlign: "center"
    },
    cell: {
      textAlign: "center"
    }
  },
  {
    Header: "部门",
    accessor: "departmentName",
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
    Header: "权限组",
    accessor: "role",
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
    Header: "最近登入IP",
    accessor: "lastLoginIp",
    disableSortBy: true,
    headCell: {
      textAlign: "center"
    },
    cell: {
      textAlign: "center"
    }
  },
  dateTimeColumn({
    Header: "最近登入时间",
    accessor: "lastLoginTime",
    disableSortBy: true
  }),
  dateTimeColumn({
    Header: "创建时间",
    accessor: "createdOn",
    disableSortBy: true
  }),
  {
    disableSortBy: true,
    Header: "状态",
    accessor: "status",
    headCell: {
      textAlign: "center"
    },
    cell: {
      textAlign: "center"
    },
    Cell: ({ value }) => {
      return statusLabel(value)
    }
  },
  {
    disableSortBy: true,
    Header: "操作",
    headCell: {
      textAlign: "center"
    },
    cell: {
      textAlign: "center"
    },
    accessor: (row) => {
      return row
    },
    Cell: ({ value }) => {
      return (
        <Set>
          <EditButton user={value} onSubmitted={fetchTable} />
          <EditPasswordButton user={value} onSubmitted={fetchTable} />
        </Set>
      )
    }
  }
]
