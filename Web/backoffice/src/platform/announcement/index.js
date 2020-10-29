import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import { Box, Set, Button, Text, Switch } from "bumbag"
import PageContent from "../../page/Content"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { useList } from "./client"
import { Code } from "./menu"
import { useTabState, Tabs, TabList, Tab, TabPanel } from "../../tab"
import { useTable, Table, pageCount } from "../../table"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { Trans } from "../../i18n"
import {
  from as statusFrom,
  Label as StatusLabel,
  InVerify,
  InVerifyName,
  Effective,
  EffectiveName,
  Expired,
  ExpiredName,
  Rejected,
  RejectedName,
  InvalidName
} from "./status"
import { AddButton, VerifyButton, EditButton, ViewButton } from "./edit"
import {
  Verify as VerifyMenu,
  View as ViewMenu,
  Update as UpdateMenu
} from "./menu"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <AsyncContentBody />
}

const AsyncContentBody = ({}) => {
  let hasVerifyAuth = useIsDisplayMenuByRoute([VerifyMenu])
  let hasViewAuth = useIsDisplayMenuByRoute([ViewMenu])
  let [statusName, setStatusName] = useState(
    hasVerifyAuth ? InVerifyName : hasViewAuth ? EffectiveName : null
  )
  let { state, setState, setPageIndex, setSortOrder } = useTable()
  let { pageIndex, pageSize, sortby, orderby } = state
  let { data = {}, isLoading, isValidating, mutate } = useList({
    page: pageIndex + 1,
    rows: pageSize,
    sortby,
    orderby,
    status:
      statusName === EffectiveName
        ? [statusFrom(EffectiveName), statusFrom(InvalidName)]
        : statusFrom(statusName)
  })

  let tab = useTabState({
    selectedId: statusName
  })

  const setStatus = (newStatusName) => () => {
    if (newStatusName === statusName) {
      mutate()
    }
    setStatusName(newStatusName)
  }

  return (
    <PageContent
      title={<Trans zh="公告管理" en="Announcements" />}
      actionBar={<ActionBar mutate={mutate} />}
    >
      <Tabs>
        <TabList {...tab}>
          {hasVerifyAuth && (
            <Tab
              tabId={InVerifyName}
              onClick={setStatus(InVerifyName)}
              {...tab}
            >
              <StatusLabel value={InVerify} />
            </Tab>
          )}

          <Tab
            tabId={EffectiveName}
            onClick={setStatus(EffectiveName)}
            {...tab}
          >
            <StatusLabel value={Effective} />
          </Tab>
          <Tab tabId={ExpiredName} onClick={setStatus(ExpiredName)} {...tab}>
            <StatusLabel value={Expired} />
          </Tab>
          <Tab tabId={RejectedName} onClick={setStatus(RejectedName)} {...tab}>
            <StatusLabel value={Rejected} />
          </Tab>
        </TabList>
        {hasVerifyAuth && (
          <TabPanel tabId={InVerifyName} {...tab}>
            <AsyncTable
              statusName={statusName}
              data={data}
              state={state}
              setPageIndex={setPageIndex}
              setSortOrder={setSortOrder}
              isLoading={isLoading}
              mutate={mutate}
            />
          </TabPanel>
        )}
        {/*<TabPanel tabId={EffectiveName} {...tab}>
          <AsyncTable
            statusName={statusName}
            data={data}
            state={state}
            setPageIndex={setPageIndex}
            setSortOrder={setSortOrder}
            isLoading={isLoading}
            mutate={mutate}
          />
        </TabPanel>
        <TabPanel tabId={ExpiredName} {...tab}>
          <AsyncTable
            statusName={statusName}
            data={data}
            state={state}
            setPageIndex={setPageIndex}
            setSortOrder={setSortOrder}
            isLoading={isLoading}
            mutate={mutate}
          />
        </TabPanel>
        <TabPanel tabId={RejectedName} {...tab}>
          <AsyncTable
            statusName={statusName}
            data={data}
            state={state}
            setPageIndex={setPageIndex}
            setSortOrder={setSortOrder}
            isLoading={isLoading}
            mutate={mutate}
          />
        </TabPanel> */}
      </Tabs>
    </PageContent>
  )
}

const ActionBar = ({ mutate }) => {
  return (
    <Box alignX="right" marginBottom="-1rem">
      <AddButton onSubmitted={mutate} />
    </Box>
  )
}

const AsyncTable = ({
  statusName,
  state,
  setPageIndex,
  setSortOrder,
  data,
  isLoading,
  mutate
}) => {
  let { pageIndex, pageSize, sortby, orderby } = state
  let columns = useColumns({ mutate, statusName })

  return (
    <Table
      columns={columns}
      data={data?.items}
      pageCount={pageCount(pageSize, data?.totalCount)}
      setPageIndex={setPageIndex}
      setSortOrder={setSortOrder}
      initialState={state}
      isLoading={isLoading}
    />
  )
}

const useColumns = ({ mutate, statusName }) => {
  let columns = [
    sequenceColumn(),
    {
      accessor: "subject",
      Header: <Trans zh="主题" en="Subject" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "createdByUsername",
      Header: <Trans zh="操作人" en="Operator" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    dateTimeColumn({
      accessor: "updatedOn",
      Header: <Trans zh="最近更新时间" en="Last Update" />,
      disableSortBy: true
    }),
    dateTimeColumn({
      accessor: "from",
      Header: <Trans zh="开始时间" en="Start" />,
      disableSortBy: true
    }),
    dateTimeColumn({
      accessor: "to",
      Header: <Trans zh="结束时间" en="End" />,
      disableSortBy: true
    }),
    {
      accessor: "status",
      Header: <Trans zh="状态" en="Status" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        //TODO: change state
        if (statusName === EffectiveName) {
          return (
            <Button>
              <StatusLabel value={value} />
            </Button>
          )
        }

        return (
          <Text.Block
            css={css`
              white-space: nowrap;
            `}
          >
            <StatusLabel value={value} />
          </Text.Block>
        )
      }
    }
  ]

  let hasVerifyAuth = useIsDisplayMenuByRoute([VerifyMenu])
  let hasViewAuth = useIsDisplayMenuByRoute([ViewMenu])
  let hasAnyMenu = hasViewAuth
  switch (statusName) {
    case InVerifyName:
      hasAnyMenu = hasVerifyAuth
      break
  }

  if (hasAnyMenu) {
    columns.push({
      id: "actions",
      disableSortBy: true,
      Header: <Trans zh="操作" en="Actions" />,
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
          <Box alignX="center">
            <Set>
              {statusName === InVerifyName && (
                <VerifyButton record={value} onSubmitted={mutate} />
              )}
              {statusName === EffectiveName && (
                <EditButton record={value} onSubmitted={mutate} />
              )}
              {(statusName === EffectiveName ||
                statusName === ExpiredName ||
                statusName === RejectedName) && <ViewButton record={value} />}
            </Set>
          </Box>
        )
      }
    })
  }
  return useMemo(() => columns, [statusName])
}
