# CivicConnect evidence screenshots

Screenshots referenced by the PED (v4.0 Final). Times shown in the load-test files are UTC.

| File | What it shows | PED section |
|---|---|---|
| 01_m3_ci_run55_green.png | CI run #55 on release/m3-rc (commit 285017d): build-test and dependency-audit green | 21, 25.2 (M3 baseline) |
| 02_m3_ci_tests_92_passed_coverage_missing_DEF-007.png | Run #55 test step: 92 passed, 0 failed, 0 skipped; "Could not find data collector 'XPlat Code Coverage'" (DEF-007 found) | 18, 20 (DEF-007) |
| 03_manual_sql_requests_rowversion.png | Manual LocalDB check: requests with status, assignee and RowVersion | 18, 21 (MC/MV) |
| 04_manual_sql_check_constraint_rejects_invalid_status.png | CHECK constraint CK_ServiceRequests_Status rejects an invalid status | 18, 21 (MC/MV) |
| 05_manual_sql_unique_index_rejects_duplicate_reference.png | Unique index IX_ServiceRequests_ReferenceNo rejects a duplicate reference number | 18, 21 (MC/MV) |
| 06_manual_two_staff_accept_one_wins.png | Two staff accept the same request: one succeeds, the other gets "already been assigned" | 18, 21 (MC-02) |
| 07_manual_sql_history_and_feedback_one_transaction.png | After the accept: one status-history row and one feedback comment, same timestamp | 18, 21 (MC-03) |
| 08_def008_max_reference_before_migration.png | Highest existing reference (CC-2026-000066) before the sequence migration | 20 (DEF-008) |
| 09_def008_sequence_starts_after_max_67.png | dbo.ServiceRequestRefSeq created with start value 67 (after the highest existing number) | 20 (DEF-008) |
| 10_loadtest_failing_run_1323Z_220_errors.png | Load test before the fix (repeat run, 13:23 UTC): 220 of 250 submits failed | 19, 20 (DEF-008) |
| 11_loadtest_after_fix_1359Z_0_errors.png | Load test after the fix (13:59 UTC): 0 errors, p95 353 ms | 19, 26 (NFR-001) |
| 12_restore_drill_1_row_counts.png | Restore drill: row count of every table, last reference number, sequence value | 28 (NFR-008, R-005) |
| 13_restore_drill_2_backup_and_verify.png | Restore drill: backup with checksum; RESTORE VERIFYONLY "backup set is valid" | 28 |
| 14_restore_drill_3_database_dropped_1703-14.png | Restore drill: database dropped at 17:03:14 (simulated total loss) | 28 |
| 15_restore_drill_4_restored_1703-26.png | Restore drill: restore complete at 17:03:26 (12 seconds) | 28 |
