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
import AsyncTable, { useTableState } from "../../table/async"
import sequenceColumn from "../../table/column/sequence"
import { AddButton, EditButton } from "./edit"
import { CopyButton } from "./copy"
import { DeleteButton } from "./delete"

export const Code = "groups"

export const Content = ({}) => {
  let { state, setState, headSortBy, headOrderBy } = useTableState({ name: "" })
  let { pageIndex, pageSize, name = "", sortBy = [], request } = state

  let fetchTable = () => {
    var promise = getRoles({
      page: pageIndex + 1,
      rows: pageSize,
      name,
      sortby: headSortBy(),
      orderby: headOrderBy()
    })

    setState({ ...state, request: promise })
  }

  let columns = useMemo(() =>
    tableColumns({
      fetchTable
    })
  )

  let async = useAsync({ promise: request })
  let { isPending } = async

  useEffect(() => {
    fetchTable()
  }, [pageIndex, pageSize, name, sortBy])

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
      initialValues={{ name: "" }}
      onSubmit={(data) => {
        search(data)
      }}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <Field
              component={InputField.Formik}
              name="name"
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("name", "")}>
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

const getRoles = (searchParams = {}) => {
  return client.get("/api/role", { searchParams })
}

//TODO: i18n
const title = () => "权限组管理"

//TODO: i18n
const searchButtonLabel = () => "搜索"

//TODO: i18n
const searchInputPlaceHolder = () => "权限组名称输入框"
const tableColumns = ({ fetchTable }) => [
  sequenceColumn(),
  {
    Header: "权限组名称",
    id: "name",
    accessor: "name",
    headCell: {
      textAlign: "center"
    },
    cell: {
      textAlign: "center"
    },
    width: 400
  },
  {
    disableSortBy: true,
    accessor: (row) => {
      return {
        id: row.id,
        name: row.name
      }
    },
    Header: "操作",
    headCell: {
      textAlign: "center"
    },
    cell: {
      textAlign: "center"
    },
    Cell: ({ value }) => {
      let { id, name } = value
      return (
        <Set>
          <EditButton id={id} onSubmitted={fetchTable} />
          <CopyButton id={id} name={name} onSubmitted={fetchTable} />
          <DeleteButton id={id} name={name} onSubmitted={fetchTable} />
        </Set>
      )
    }
  }
]
