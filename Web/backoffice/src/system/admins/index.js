import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Heading, Stack, Button, Set, Text } from "bumbag"
import PageContent from "../../page/Content"
import { AddButton, EditButton, EditPasswordButton } from "./edit"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { AdminStatusLabel } from "./AdminStatus"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"
import { useAdminList } from "./client"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { FormProvider, useForm } from "react-hook-form"
import { InputField } from "../../input/Input"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
  let { state, setState, setPageIndex, setSortOrder } = useTable({
    username: ""
  })

  let { pageIndex, pageSize, username = "" } = state

  let { data, isLoading, mutate } = useAdminList(toApiParams(state))
  let columns = useColumns({ mutate, items: data?.items })

  return (
    <PageContent
      title={<Trans zh="管理员列表" en="Admins" />}
      actionBar={
        <>
          <SearchBar
            search={(data) => {
              let newState = {
                pageIndex: 0,
                username: data?.username
              }
              if (equals(newState, { pageIndex, username })) {
                mutate()
              }
              setState(newState)
            }}
            isPending={isLoading}
          />
          <AddButton onSubmitted={mutate} />
        </>
      }
    >
      <Table
        controlledState={state}
        columns={columns}
        data={data?.items}
        pageCount={pageCount(pageSize, data?.totalCount)}
        setPageIndex={setPageIndex}
        setSortOrder={setSortOrder}
        isLoading={isLoading}
      />
    </PageContent>
  )
}

const SearchBar = ({ search, isLoading, ...props }) => {
  let formMethods = useForm({
    defaultValues: { username: "" }
  })

  const onSubmit = search

  return (
    <FormProvider {...formMethods}>
      <form onSubmit={formMethods.handleSubmit(onSubmit)}>
        <Set>
          <InputField
            name="username"
            addonAfter={
              <Button onClick={() => formMethods.setValue("username", "")}>
                X
              </Button>
            }
            placeholder={useSearchInputPlaceHolder()}
            autocomplete="off"
          />
          <SearchButton isLoading={isLoading} />
        </Set>
      </form>
    </FormProvider>
  )
}

const useSearchInputPlaceHolder = () =>
  useTrans({
    zh: "管理员用户名输入框",
    en: "Input admin user name"
  })

const useColumns = ({ mutate, items = [] }) => {
  let hasAnyMenu = useHasSubMenuByRoute()

  return useMemo(() => {
    let columns = [
      sequenceColumn(),
      {
        Header: <Trans zh="用户名" en="Username" />,
        accessor: "username",
        disableSortBy: true,
        headCell: {
          textAlign: "center"
        },
        cell: {
          textAlign: "center"
        }
      },
      {
        Header: <Trans zh="姓名" en="Name" />,
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
        Header: <Trans zh="部门" en="Department" />,
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
        Header: <Trans zh="权限组" en="Role" />,
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
        Header: <Trans zh="最近登入IP" en="Last Login IP" />,
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
        Header: <Trans zh="最近登入时间" en="Last Login Time" />,
        accessor: "lastLoginTime",
        disableSortBy: true
      }),
      dateTimeColumn({
        Header: <Trans zh="创建时间" en="Created On" />,
        accessor: "createdOn",
        disableSortBy: true
      }),
      {
        disableSortBy: true,
        Header: <Trans zh="状态" en="Status" />,
        accessor: "status",
        headCell: {
          textAlign: "center"
        },
        cell: {
          textAlign: "center"
        },
        Cell: ({ value }) => (
          <Text.Block
            css={css`
              white-space: nowrap;
            `}
          >
            <AdminStatusLabel value={value} />
          </Text.Block>
        )
      }
    ]

    if (hasAnyMenu) {
      columns.push({
        id: "actions",
        accessor: (row) => row,
        disableSortBy: true,
        Header: <Trans zh="操作" en="Actions" />,
        headCell: {
          textAlign: "center"
        },
        cell: {
          textAlign: "center"
        },
        Cell: ({ value }) => {
          return (
            <Set orientation="vertical" alignX="center">
              <EditButton user={value} onSubmitted={mutate} />
              <EditPasswordButton user={value} onSubmitted={mutate} />
            </Set>
          )
        }
      })
    }
    return columns
  }, [JSON.stringify(items)])
}
