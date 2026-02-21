---
name: auditAlgorithmImplementation
---
# Audit Algorithm Implementation Across Multiple Files

Review the implementation of ${ALGORITHM_NAME} across all related files in the `${DIRECTORY_PATH}` directory for correctness and consistency.

## Your Task

1. **Scan all files** in the specified directory to understand the algorithm's variants and specializations
2. **Verify mathematical correctness** by checking:
   - Formula implementations match the algorithm specification
   - Operator precedence and grouping are correct (e.g., not applying coefficients to wrong variables)
   - Variable usage is semantically correct (e.g., not confusing position with velocity)
3. **Check for logical consistency** across variants:
   - Minimize vs. Maximize methods use consistent logic
   - Initial state selection matches the optimization goal (e.g., GetMin for minimize, GetMax for maximize)
   - All specializations (1D, 2D, multi-dimensional) apply the algorithm correctly
4. **Validate parameter usage**:
   - Coefficients and weights are applied in the right order
   - Boundary conditions are respected
5. **Create a comprehensive plan** listing:
   - All identified errors with their locations and impact
   - The correct formulas/logic that should be used
   - Steps to fix each issue systematically
6. **Apply fixes** across all files, ensuring consistency
7. **Verify compilation** after changes

## Deliverables

- List of errors found with file references and line numbers
- Explanation of each error's impact on algorithm behavior
- Applied fixes with compilation verification
